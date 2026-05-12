using MediatR;

namespace SkillLink.Application.UseCases.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand : IRequest<int>
    {
        public int ReviewerId { get; init; }
        public int RevieweeId { get; init; }
        public int? SessionId { get; init; }
        public int? SkillTradeId { get; init; }
        public int Rating { get; init; }
        public string? Comment { get; init; }
    }
}
