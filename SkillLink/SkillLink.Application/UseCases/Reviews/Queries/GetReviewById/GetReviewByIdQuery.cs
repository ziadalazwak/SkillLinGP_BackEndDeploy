using MediatR;

namespace SkillLink.Application.UseCases.Reviews.Queries.GetReviewById
{
    public record GetReviewByIdQuery(int Id) : IRequest<ReviewDto?>;
}
