namespace SkillLink.Application.UseCases.Users.DTOs
{
    public record UserProfileDto(
        int UserId,
        string FullName,
        string Email,
        string? PhoneNumber,
        string? Bio,
        int CreditBalance,
        string? ProfilePictureUrl,
        double AverageRating,
        int TotalReviewsCount,
        bool isadmin,
        DateTime CreatedAt,
        IEnumerable<PublicUserSkillDto> Skills
    );

    public record PublicUserSkillDto(
        int SkillId,
        string Title,
        string CategoryName,
        string Level,
        bool IsOffering
    );

    public record PublicUserProfileDto(
        int UserId,
        string FullName,
        string? Bio,
        string? ProfilePictureUrl,
        double AverageRating,
        int TotalReviewsCount,
        DateTime CreatedAt,
        IEnumerable<PublicUserSkillDto> OfferedSkills
    );

    public record ReviewDto(
        int ReviewId,
        int ReviewerId,
        string ReviewerName,
        int Rating,
        string? Comment,
        DateTime CreatedAt
    );
}
