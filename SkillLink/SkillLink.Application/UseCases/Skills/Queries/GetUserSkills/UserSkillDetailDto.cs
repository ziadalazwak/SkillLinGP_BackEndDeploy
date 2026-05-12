using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public record UserSkillDetailDto(
        int SkillId,
        string Title,
        string? Description,
        string CategoryName,
        string? ImageUrl,
        SkillLevel SkillLevel,
        bool IsOffering,
        ExchangeMode ExchangeMode,
        int? DurationMinutes,
        int? CreditCost,
        string? DeliveryMode,
        string? Notes
    );
}
