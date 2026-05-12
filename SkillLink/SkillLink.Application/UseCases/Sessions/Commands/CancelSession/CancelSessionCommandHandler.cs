using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Commands.CancelSession
{
    public class CancelSessionCommandHandler : IRequestHandler<CancelSessionCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CancelSessionCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null) return false;

            if (session.ProviderId != request.UserId && session.RequesterId != request.UserId)
                throw new UnauthorizedAccessException("Only participants can cancel the session.");

            if (session.Status == SessionStatus.Completed || session.Status == SessionStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel a session that is already completed or cancelled.");

            session.Status = SessionStatus.Cancelled;

            if (session.ProviderId == request.UserId)
                session.ProviderNote = request.Reason ?? session.ProviderNote;
            else
                session.RequesterNote = request.Reason ?? session.RequesterNote;

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
                    Description  = "Credit refund on session cancellation"
                };

                await _context.CreditTransactions.AddAsync(releaseTx, cancellationToken);
                _context.DomainUsers.Update(requester);
            }

            _context.Sessions.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            var otherUserId = request.UserId == session.ProviderId ? session.RequesterId : session.ProviderId;
            await _notificationService.SendNotificationAsync(
                otherUserId,
                NotificationType.SessionRejected,
                "Session Cancelled",
                "A session you were part of has been cancelled." + (string.IsNullOrEmpty(request.Reason) ? "" : $" Reason: {request.Reason}"),
                $"/sessions/{session.Id}",
                cancellationToken);

            return true;
        }
    }
}
