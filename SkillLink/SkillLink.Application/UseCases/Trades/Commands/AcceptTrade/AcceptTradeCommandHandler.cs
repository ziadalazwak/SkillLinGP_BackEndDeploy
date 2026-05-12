using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Commands.AcceptTrade
{
    public class AcceptTradeCommandHandler : IRequestHandler<AcceptTradeCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public AcceptTradeCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(AcceptTradeCommand request, CancellationToken cancellationToken)
        {
            var trade = await _context.SkillTrades
                .FirstOrDefaultAsync(t => t.Id == request.TradeId, cancellationToken);

            if (trade == null) return false;

            if (trade.ReceiverId != request.ReceiverId)
            {
                throw new UnauthorizedAccessException("Only the receiver can accept the trade.");
            }

            if (trade.Status != TradeStatus.Pending)
            {
                throw new InvalidOperationException("Trade is not in Pending state.");
            }

            trade.Status = TradeStatus.Accepted;
            trade.MeetingLink = $"https://meet.jit.si/SkillLink-Trade-{trade.Id}-{Guid.NewGuid():N}";

            _context.SkillTrades.Update(trade);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                trade.OffererId,
                NotificationType.TradeAccepted,
                "Trade Accepted",
                "Your skill trade request has been accepted! A meeting link has been generated.",
                $"/trades/{trade.Id}",
                cancellationToken);

            return true;
        }
    }
}
