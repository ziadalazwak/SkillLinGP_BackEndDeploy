using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public enum TradeStatus
    {
        Pending = 0,    // Proposal sent (FR4.1)
        Accepted = 1,   // Both confirmed (FR4.2)
        Rejected = 2,
        Completed = 3,  // Trade executed and logged (FR4.3)
        Cancelled = 4
    }

    /// <summary>
    /// Represents a direct skill-for-skill exchange between two users (FR4.1–FR4.4)
    /// </summary>
    public class SkillTrade:BaseEntity
    {
        /// <summary>User initiating the trade</summary>
        [ForeignKey(nameof(Offerer))]
        public int OffererId { get; set; }

        /// <summary>User receiving the trade proposal</summary>
        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }

        /// <summary>Skill the Offerer is offering</summary>
        [ForeignKey(nameof(OfferedSkill))]
        public int OfferedSkillId { get; set; }

        /// <summary>Skill the Offerer wants in return</summary>
        [ForeignKey(nameof(RequestedSkill))]
        public int RequestedSkillId { get; set; }

        public TradeStatus Status { get; set; } = TradeStatus.Pending;

        [MaxLength(500)]
        public string? Message { get; set; }

        public string MeetingLink { get; set; } = string.Empty;

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public User Offerer { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public Skill OfferedSkill { get; set; } = null!;
        public Skill RequestedSkill { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
