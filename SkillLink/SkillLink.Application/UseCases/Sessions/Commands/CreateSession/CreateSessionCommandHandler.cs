using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Commands.CreateSession
{
    public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, int>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CreateSessionCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.RequesterId == request.ProviderId)
                throw new InvalidOperationException("Cannot request a session with yourself.");

            // ── Validate duration (30–180 minutes) ───────────────────────────
            if (request.RequestedDurationMinutes < 30 || request.RequestedDurationMinutes > 180)
                throw new InvalidOperationException("Session duration must be between 30 and 180 minutes.");

            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.SkillId && s.IsActive, cancellationToken);
            if (skill == null) throw new KeyNotFoundException("Skill not found or inactive.");

            var providerUserSkill = await _context.UserSkills
                .FirstOrDefaultAsync(
                    us => us.UserId == request.ProviderId
                       && us.SkillId == request.SkillId
                       && us.IsOffering
                       && !us.IsDeleted,
                    cancellationToken);
            if (providerUserSkill == null)
                throw new InvalidOperationException("The specified provider does not offer this skill.");

            var requester = await _context.DomainUsers
                .FindAsync(new object[] { request.RequesterId }, cancellationToken);
            if (requester == null) throw new KeyNotFoundException("Requester not found.");

            // ── Compute credit cost: 1 credit per hour (always a whole number) ───
            bool isCreditBased = providerUserSkill.ExchangeMode == ExchangeMode.CreditBased
                               || providerUserSkill.ExchangeMode == ExchangeMode.Both;

            int? creditCost = null;
            if (isCreditBased)
            {
                // Round to nearest whole hour (e.g. 30–89 min = 1, 90–149 min = 2, 150–180 min = 3)
                // MidpointRounding.AwayFromZero ensures 90 min rounds to 2 (not 1)
                creditCost = (int)Math.Round(request.RequestedDurationMinutes / 60m, MidpointRounding.AwayFromZero);

                // ── Hold (escrow) the credits immediately ─────────────────────
                requester.HoldCredits(creditCost.Value);

                var holdTx = new CreditTransaction
                {
                    UserId       = requester.Id,
                    Amount       = -creditCost.Value,
                    BalanceAfter = requester.CreditBalance,
                    Type         = TransactionType.CreditHold,
                    Description  = $"Credit hold for session booking ({request.RequestedDurationMinutes} min)"
                };
                await _context.CreditTransactions.AddAsync(holdTx, cancellationToken);
                _context.DomainUsers.Update(requester);
            }

            var session = new Session
            {
                SkillId         = request.SkillId,
                RequesterId     = request.RequesterId,
                ProviderId      = request.ProviderId,
                ScheduledAt     = request.ScheduledAt,
                DurationMinutes = request.RequestedDurationMinutes,
                CreditCost      = creditCost,
                RequesterNote   = request.Note,
                Status          = SessionStatus.Pending
            };

            await _context.Sessions.AddAsync(session, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                session.ProviderId,
                NotificationType.SessionRequest,
                "New Session Request",
                $"You have a new session request for \"{skill.Title}\" ({request.RequestedDurationMinutes} min).",
                $"/sessions/{session.Id}",
                cancellationToken);

            return session.Id;
        }
    }
}
