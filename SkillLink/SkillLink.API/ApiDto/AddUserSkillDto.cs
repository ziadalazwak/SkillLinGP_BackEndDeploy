namespace SkillLink.API.ApiDto
{
    public class AddUserSkillDto
    {
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public SkillLink.Domain.Models.SkillLevel Level { get; set; }
        public SkillLink.Domain.Models.ExchangeMode ExchangeMode { get; set; } = SkillLink.Domain.Models.ExchangeMode.CreditBased;

        /// <summary>Session duration in minutes this provider offers (30–180). Credit cost is computed automatically as DurationMinutes / 60.</summary>
        public int? DurationMinutes { get; set; }

        public string? DeliveryMode { get; set; }
        public string? Notes { get; set; }
    }
}
