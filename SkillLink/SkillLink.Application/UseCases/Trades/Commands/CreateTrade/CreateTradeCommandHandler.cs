using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Commands.CreateTrade
{
    public class CreateTradeCommandHandler : IRequestHandler<CreateTradeCommand, int>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CreateTradeCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(CreateTradeCommand request, CancellationToken cancellationToken)
        {
            if (request.OffererId == request.ReceiverId)
            {
                throw new InvalidOperationException("Cannot trade with yourself.");
            }

            // Verify both skills exist and are active
            var activeSkillsCount = await _context.Skills
                .Where(s => (s.Id == request.OfferedSkillId || s.Id == request.RequestedSkillId) && s.IsActive)
                .CountAsync(cancellationToken);

            if (activeSkillsCount < (request.OfferedSkillId == request.RequestedSkillId ? 1 : 2))
            {
                throw new KeyNotFoundException("One or both skills were not found or are inactive.");
            }

            // Verify offerer offers the offered skill
            var offererValid = await _context.UserSkills
                .AnyAsync(us => us.UserId == request.OffererId && us.SkillId == request.OfferedSkillId && us.IsOffering, cancellationToken);
            if (!offererValid) throw new InvalidOperationException("Offerer does not offer the specified skill.");

            // Verify receiver offers the requested skill
            var receiverValid = await _context.UserSkills
                .AnyAsync(us => us.UserId == request.ReceiverId && us.SkillId == request.RequestedSkillId && us.IsOffering, cancellationToken);
            if (!receiverValid) throw new InvalidOperationException("Receiver does not offer the requested skill.");

            var trade = new SkillTrade
            {
                OffererId = request.OffererId,
                ReceiverId = request.ReceiverId,
                OfferedSkillId = request.OfferedSkillId,
                RequestedSkillId = request.RequestedSkillId,
                Status = TradeStatus.Pending
            };

            await _context.SkillTrades.AddAsync(trade, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                trade.ReceiverId,
                NotificationType.TradeRequest,
                "New Skill Trade Request",
                "You have received a new skill trade request.",
                $"/trades/{trade.Id}",
                cancellationToken);

            return trade.Id;
        }
    }
}
