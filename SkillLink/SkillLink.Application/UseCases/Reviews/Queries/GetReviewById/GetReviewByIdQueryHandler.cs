using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Reviews.Queries.GetReviewById
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetReviewByIdQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewDto?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (review == null) return null;

            return new ReviewDto(
                review.Id,
                review.ReviewerId,
                review.RevieweeId,
                review.SessionId,
                review.SkillTradeId,
                review.Rating,
                review.Comment,
                review.CreatedAt
            );
        }
    }
}
