using MediatR;
using SkillLink.Application.Common;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetUserReviews
{
    public record GetUserReviewsQuery(int UserId, int Page = 1, int PageSize = 10) : IRequest<PaginatedList<ReviewDto>>;
}
