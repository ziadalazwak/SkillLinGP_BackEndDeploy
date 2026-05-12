using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Commands.RejectSession
{
    public class RejectSessionCommandHandler : IRequestHandler<RejectSessionCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public RejectSessionCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(RejectSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null) return false;

            if (session.ProviderId != request.ProviderId)
                throw new UnauthorizedAccessException("Only the provider can reject the session.");

            if (session.Status != SessionStatus.Pending)
                throw new InvalidOperationException("Session is not in Pending state.");

            session.Status = SessionStatus.Rejected;
            session.ProviderNote = request.Reason;

            // ── Refund held credits to requester ──────────────────────────────
            if (session.CreditCost.HasValue && session.CreditCost.Value > 0)
            {
                var requester = await _context.DomainUsers
                    .FirstAsync(u => u.Id == session.RequesterId, cancellationToken);

                requester.ReleaseHeldCredits(session.CreditCost.Value);

                var releaseTx = new CreditTransaction
                {
                    UserId       = requester.Id,
                    SessionId    = session.Id,
                    Amount       = session.CreditCost.Value,
                    BalanceAfter = requester.CreditBalance,
                    Type         = TransactionType.CreditRelease,
                    Description  = "Credit refund on session rejection"
                };

                await _context.CreditTransactions.AddAsync(releaseTx, cancellationToken);
                _context.DomainUsers.Update(requester);
            }

            _context.Sessions.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                session.RequesterId,
                NotificationType.SessionRejected,
                "Session Rejected",
                "Your session request has been rejected." + (string.IsNullOrEmpty(request.Reason) ? "" : $" Reason: {request.Reason}"),
                $"/sessions/{session.Id}",
                cancellationToken);

            return true;
        }
    }
}
