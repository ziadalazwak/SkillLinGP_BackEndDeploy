using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public enum NotificationType
    {
        SessionRequest = 0,
        SessionAccepted = 1,
        SessionRejected = 2,
        SessionCompleted = 3,
        TradeRequest = 4,
        TradeAccepted = 5,
        TradeRejected = 6,
        TradeCompleted = 7,
        NewMessage = 8,
        NewReview = 9,
        CreditReceived = 10,
        AdminAlert = 11
    }

    /// <summary>
    /// In-app notifications to keep users informed of system activity
    /// </summary>
    public class Notification:BaseEntity
    {
      

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public NotificationType Type { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Body { get; set; }

        /// <summary>Deep-link within the app (e.g. /sessions/42)</summary>
        public string? ActionUrl { get; set; }

        public bool IsRead { get; set; } = false;

   

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
