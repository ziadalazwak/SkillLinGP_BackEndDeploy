using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.Common;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetUserReviews
{
    public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQuery, PaginatedList<ReviewDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUserReviewsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ReviewDto>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Reviews
                .AsNoTracking()
                .Include(r => r.Reviewer)
                .Where(r => r.RevieweeId == request.UserId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReviewDto(
                    r.Id,
                    r.ReviewerId,
                    r.Reviewer.FullName ?? "Unknown User",
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedList<ReviewDto>(reviews, totalCount, request.Page, request.PageSize);
        }
    }
}
