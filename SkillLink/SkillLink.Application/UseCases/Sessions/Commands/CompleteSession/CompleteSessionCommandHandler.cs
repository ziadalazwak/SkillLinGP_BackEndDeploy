using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Commands.CompleteSession
{
    public class CompleteSessionCommandHandler : IRequestHandler<CompleteSessionCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CompleteSessionCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(CompleteSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null) return false;

            if (session.ProviderId != request.UserId && session.RequesterId != request.UserId)
                throw new UnauthorizedAccessException("Only participants can complete the session.");

            if (session.Status != SessionStatus.Accepted)
                throw new InvalidOperationException("Session must be in Accepted state to be completed.");

            session.Status = SessionStatus.Completed;
            session.CompletedAt = DateTime.UtcNow;

            // ── Settle held credits (escrow → provider) ───────────────────────
            if (session.CreditCost.HasValue && session.CreditCost.Value > 0)
            {
                var requester = await _context.DomainUsers
                    .FirstAsync(u => u.Id == session.RequesterId, cancellationToken);
                var provider = await _context.DomainUsers
                    .FirstAsync(u => u.Id == session.ProviderId, cancellationToken);

                // Settle: remove from requester's held balance (credits already left available balance on hold)
                requester.SettleHeldCredits(session.CreditCost.Value);
                var spentTx = new CreditTransaction
                {
                    UserId       = requester.Id,
                    SessionId    = session.Id,
                    Amount       = -session.CreditCost.Value,
                    BalanceAfter = requester.CreditBalance,
                    Type         = TransactionType.Spent,
                    Description  = "Session payment settled"
                };

                // Credit the provider
                provider.AdjustCreditBalance(session.CreditCost.Value);
                var earnTx = new CreditTransaction
                {
                    UserId       = provider.Id,
                    SessionId    = session.Id,
                    Amount       = session.CreditCost.Value,
                    BalanceAfter = provider.CreditBalance,
                    Type         = TransactionType.Earned,
                    Description  = "Session earnings received"
                };

                _context.CreditTransactions.AddRange(spentTx, earnTx);
                _context.DomainUsers.UpdateRange(requester, provider);
            }

            _context.Sessions.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            var otherUserId = request.UserId == session.ProviderId ? session.RequesterId : session.ProviderId;
            await _notificationService.SendNotificationAsync(
                otherUserId,
                NotificationType.SessionCompleted,
                "Session Completed",
                "A session you participated in has been marked as completed.",
                $"/sessions/{session.Id}",
                cancellationToken);

            if (session.CreditCost.HasValue && session.CreditCost.Value > 0)
            {
                await _notificationService.SendNotificationAsync(
                    session.ProviderId,
                    NotificationType.CreditReceived,
                    "Credits Received",
                    $"You earned {session.CreditCost.Value} credits for completing a session.",
                    $"/sessions/{session.Id}",
                    cancellationToken);
            }

            return true;
        }
    }
}
