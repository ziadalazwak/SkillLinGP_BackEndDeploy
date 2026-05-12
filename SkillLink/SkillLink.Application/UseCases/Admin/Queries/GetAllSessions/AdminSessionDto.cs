using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSessions
{
    public class AdminSessionDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillTitle { get; set; } = string.Empty;
        public AdminSessionUserDto Mentor { get; set; } = null!;
        public AdminSessionUserDto Learner { get; set; } = null!;
        public SessionStatus Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? CreditCost { get; set; }
        public string? MeetingLink { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
