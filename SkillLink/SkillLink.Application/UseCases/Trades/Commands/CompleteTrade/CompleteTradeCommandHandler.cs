using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Commands.CompleteTrade
{
    public class CompleteTradeCommandHandler : IRequestHandler<CompleteTradeCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CompleteTradeCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(CompleteTradeCommand request, CancellationToken cancellationToken)
        {
            var trade = await _context.SkillTrades
                .FirstOrDefaultAsync(t => t.Id == request.TradeId, cancellationToken);

            if (trade == null) return false;

            if (trade.OffererId != request.UserId && trade.ReceiverId != request.UserId)
            {
                throw new UnauthorizedAccessException("Only participants can complete the trade.");
            }

            if (trade.Status != TradeStatus.Accepted)
            {
                throw new InvalidOperationException("Trade must be in Accepted state to be completed.");
            }

            trade.Status = TradeStatus.Completed;
            trade.CompletedAt = DateTime.UtcNow;

            _context.SkillTrades.Update(trade);
            await _context.SaveChangesAsync(cancellationToken);

            var otherUserId = request.UserId == trade.OffererId ? trade.ReceiverId : trade.OffererId;
            await _notificationService.SendNotificationAsync(
                otherUserId,
                NotificationType.TradeCompleted,
                "Trade Completed",
                "A skill trade you participated in has been marked as completed.",
                $"/trades/{trade.Id}",
                cancellationToken);

            return true;
        }
    }
}
