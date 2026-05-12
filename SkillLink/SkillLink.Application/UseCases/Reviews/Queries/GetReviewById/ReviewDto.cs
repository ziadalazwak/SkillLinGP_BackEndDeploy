namespace SkillLink.Application.UseCases.Reviews.Queries.GetReviewById
{
    public record ReviewDto(
        int Id,
        int ReviewerId,
        int RevieweeId,
        int? SessionId,
        int? SkillTradeId,
        int Rating,
        string? Comment,
        DateTime CreatedAt
    );
}
