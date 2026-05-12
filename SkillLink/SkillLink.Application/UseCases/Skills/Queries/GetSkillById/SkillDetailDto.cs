using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Skills.Queries.GetSkillById
{
    public class ProviderOfferDto
    {
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public SkillLevel Level { get; set; }
        public ExchangeMode ExchangeMode { get; set; }
        public int? CreditCost { get; set; }
        public int? DurationMinutes { get; set; }
        public string? DeliveryMode { get; set; }
        public string? Notes { get; set; }
    }

    public record SkillDetailDto(
        int Id,
        string Title,
        string? Description,
        string CategoryName,
        string ImageUrl,
        DateTime CreatedAt,
        List<ProviderOfferDto> Providers
    );
}
