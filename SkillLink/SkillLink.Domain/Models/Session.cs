using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public enum SessionStatus
    {
        Pending = 0,       // Request sent, waiting for provider
        Accepted = 1,      // Provider accepted
        Rejected = 2,      // Provider rejected
        Completed = 3,     // Both confirmed completion (FR6.5)
        Cancelled = 4      // Cancelled by either party
    }

    /// <summary>
    /// Represents a skill session request and its lifecycle (FR6.1–FR6.5)
    /// </summary>
    public class Session: BaseEntity
    {


    
        public int SkillId { get; set; }

        /// <summary>The user who is providing the skill</summary>
       
        public int ProviderId { get; set; }

        /// <summary>The user who requested the skill</summary>
  
        public int RequesterId { get; set; }

        public SessionStatus Status { get; set; } = SessionStatus.Pending;

        public DateTime? ScheduledAt { get; set; }   // FR6.3 – time slot

        public int? DurationMinutes { get; set; }

        public string MeetingLink { get; set; } = string.Empty;
        public int? CreditCost { get; set; }

        [MaxLength(500)]
        public string? RequesterNote { get; set; }

        [MaxLength(500)]
        public string? ProviderNote { get; set; }

        

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public Skill Skill { get; set; } = null!;
        public User Provider { get; set; } = null!;
        public User Requester { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];
    }

}
