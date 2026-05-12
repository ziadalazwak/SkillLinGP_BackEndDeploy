using System;

namespace SkillLink.API.ApiDto
{
    public class CreateSessionDto
    {
        public int SkillId { get; set; }
        public int ProviderId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string? Note { get; set; }

        /// <summary>
        /// Duration the requester wants (30–180 minutes). Required for credit-based sessions.
        /// </summary>
        public int RequestedDurationMinutes { get; set; }
    }

    public class SessionReasonDto
    {
        public string? Reason { get; set; }
    }

    // SessionActionDto is no longer needed since userId comes from token,
    // but kept for backward compatibility.
    public class SessionActionDto
    {
    }
}
