using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SkillLink.Domain.Models
{
    public enum SkillLevel
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2
    }

    public enum ExchangeMode
    {
        CreditBased = 0,
        SkillTrade = 1,
        Both = 2
    }
    public class UserSkill: BaseEntity
    {
    

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Skill))]
        public int SkillId { get; set; }

        public SkillLevel Level { get; set; } = SkillLevel.Beginner;

        public ExchangeMode ExchangeMode { get; set; } = ExchangeMode.CreditBased;

        /// <summary>Default session duration in minutes offered by this provider (30–180).</summary>
        public int? DurationMinutes { get; set; }

        [MaxLength(50)]
        public string? DeliveryMode { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsOffering { get; set; } = true;

        // Navigation properties
        public User User { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }

    /// <summary>
    /// Represents a skill listing offered by a user (FR2.1–FR2.3, FR5.1)
    /// </summary>
    public class Skill : BaseEntity
    {



        public string Title { get; set; } = string.Empty;


        public string? Description { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }
 

        public string? EmbeddingJson { get; set; }

        // Navigation properties
        public SkillCategory Category { get; set; } = null!;
        public ICollection<UserSkill> UserSkills { get; set; } = [];
      
        public ICollection<Session> Sessions { get; set; } = [];
        public ICollection<SkillTrade> TradesOffered { get; set; } = [];
        public ICollection<SkillTrade> TradesRequested { get; set; } = [];
        public static Skill Create(
          string title,
          string? description,
          int categoryId,
          string? imageUrl)
        {
            return new Skill
            {
                Title          = title,
                Description    = description,
                CategoryId     = categoryId,
                ImageUrl       = imageUrl,
            };
        }

    }
}