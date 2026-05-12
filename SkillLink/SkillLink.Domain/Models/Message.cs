using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public class Message: BaseEntity
    {
 

        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }

        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }

        /// <summary>Optional — message scoped to a specific session</summary>
        [ForeignKey(nameof(Session))]
        public int? SessionId { get; set; }

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>URL of an attached file (FR9.2)</summary>
        public string? AttachmentUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public Session? Session { get; set; }
    }
}
