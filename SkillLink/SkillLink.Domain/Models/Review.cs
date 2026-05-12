using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public class Review: BaseEntity
    {
     

    
        public int ReviewerId { get; set; }

        public int RevieweeId { get; set; }

        /// <summary>Optional — review linked to a credit-based session</summary>

        public int? SessionId { get; set; }

        /// <summary>Optional — review linked to a skill trade</summary>

        public int? SkillTradeId { get; set; }

        public int Rating { get; set; }

     
        public string? Comment { get; set; }

    

        // Navigation properties
        public User Reviewer { get; set; } = null!;
        public User Reviewee { get; set; } = null!;
        public Session? Session { get; set; }
        public SkillTrade? SkillTrade { get; set; }
    }
}
