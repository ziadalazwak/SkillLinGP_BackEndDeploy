using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, int>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public CreateReviewCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            if (request.SessionId == null && request.SkillTradeId == null)
            {
                throw new ArgumentException("Review must be linked to a Session or a SkillTrade.");
            }

            // Check duplicate review
            var exists = await _context.Reviews.AnyAsync(r =>
                r.ReviewerId == request.ReviewerId &&
                r.RevieweeId == request.RevieweeId &&
                ((request.SessionId != null && r.SessionId == request.SessionId) ||
                 (request.SkillTradeId != null && r.SkillTradeId == request.SkillTradeId)),
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("You have already reviewed this interaction.");
            }

            if (request.SessionId.HasValue)
            {
                var session = await _context.Sessions.FindAsync(new object[] { request.SessionId.Value }, cancellationToken);
                if (session == null) throw new KeyNotFoundException("Session not found.");
                if (session.Status != SessionStatus.Completed) throw new InvalidOperationException("Cannot review an incomplete session.");
                if (session.RequesterId != request.ReviewerId && session.ProviderId != request.ReviewerId)
                {
                    throw new UnauthorizedAccessException("You did not participate in this session.");
                }
            }

            if (request.SkillTradeId.HasValue)
            {
                var trade = await _context.SkillTrades.FindAsync(new object[] { request.SkillTradeId.Value }, cancellationToken);
                if (trade == null) throw new KeyNotFoundException("Trade not found.");
                if (trade.Status != TradeStatus.Completed) throw new InvalidOperationException("Cannot review an incomplete trade.");
                if (trade.OffererId != request.ReviewerId && trade.ReceiverId != request.ReviewerId)
                {
                    throw new UnauthorizedAccessException("You did not participate in this trade.");
                }
            }

            var review = new Review
            {
                ReviewerId = request.ReviewerId,
                RevieweeId = request.RevieweeId,
                SessionId = request.SessionId,
                SkillTradeId = request.SkillTradeId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                review.RevieweeId,
                NotificationType.NewReview,
                "New Review Received",
                $"You received a {review.Rating}-star review." + (!string.IsNullOrEmpty(review.Comment) ? $" \"{review.Comment}\"" : ""),
                $"/reviews/{review.Id}",
                cancellationToken);

            return review.Id;
        }
    }
}
