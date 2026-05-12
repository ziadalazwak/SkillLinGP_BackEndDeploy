namespace SkillLink.Application.UseCases.Users.DTOs
{
    public record RecommendedSkillDto(
        int SkillId,
        /// <summary>The UserSkill (provider-offering) record ID — use this to identify the exact provider offering.</summary>
        int UserSkillId,
        string Title,
        string CategoryName,
        string Description,
        string? ImageUrl,
        double MatchScore,
        string ProviderName,
        int providerId,
        int? CreditCost,
        int? sessionDuration,
        Domain.Models.ExchangeMode ExchangeMode,
        Domain.Models.SkillLevel Level
    );
}
