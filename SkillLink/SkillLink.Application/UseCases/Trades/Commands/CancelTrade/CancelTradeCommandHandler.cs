using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Commands.CancelTrade
{
    public class CancelTradeCommandHandler : IRequestHandler<CancelTradeCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CancelTradeCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(CancelTradeCommand request, CancellationToken cancellationToken)
        {
            var trade = await _context.SkillTrades
                .FirstOrDefaultAsync(t => t.Id == request.TradeId, cancellationToken);

            if (trade == null) return false;

            if (trade.OffererId != request.UserId && trade.ReceiverId != request.UserId)
            {
                throw new UnauthorizedAccessException("Only participants can cancel the trade.");
            }

            if (trade.Status == TradeStatus.Completed || trade.Status == TradeStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot cancel a trade that is already completed or cancelled.");
            }

            trade.Status = TradeStatus.Cancelled;

            _context.SkillTrades.Update(trade);
            await _context.SaveChangesAsync(cancellationToken);

            var otherUserId = request.UserId == trade.OffererId ? trade.ReceiverId : trade.OffererId;
            await _notificationService.SendNotificationAsync(
                otherUserId,
                NotificationType.TradeRejected,
                "Trade Cancelled",
                "A skill trade you were part of has been cancelled.",
                $"/trades/{trade.Id}",
                cancellationToken);

            return true;
        }
    }
}
