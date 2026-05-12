using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Commands.RejectTrade
{
    public class RejectTradeCommandHandler : IRequestHandler<RejectTradeCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public RejectTradeCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(RejectTradeCommand request, CancellationToken cancellationToken)
        {
            var trade = await _context.SkillTrades
                .FirstOrDefaultAsync(t => t.Id == request.TradeId, cancellationToken);

            if (trade == null) return false;

            if (trade.ReceiverId != request.ReceiverId)
            {
                throw new UnauthorizedAccessException("Only the receiver can reject the trade.");
            }

            if (trade.Status != TradeStatus.Pending)
            {
                throw new InvalidOperationException("Trade is not in Pending state.");
            }

            trade.Status = TradeStatus.Rejected;

            _context.SkillTrades.Update(trade);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                trade.OffererId,
                NotificationType.TradeRejected,
                "Trade Rejected",
                "Your skill trade request has been rejected.",
                $"/trades/{trade.Id}",
                cancellationToken);

            return true;
        }
    }
}
