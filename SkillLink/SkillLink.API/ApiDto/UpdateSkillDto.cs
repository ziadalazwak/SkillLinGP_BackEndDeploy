using Microsoft.AspNetCore.Http;
using SkillLink.Domain.Models;

namespace SkillLink.API.ApiDto
{
    public class UpdateSkillDto
    {
        public int ProviderId { get; set; } // Should ideally be from JWT
        public string? Title { get; set; }
        public string? Description { get; set; }
        public SkillLevel? SkillLevel { get; set; }
        public int? CreditCost { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile? Image { get; set; }
    }
}
