using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Domain.Models
{
    public enum TransactionType
    {
        InitialBonus = 0,    // FR3.1 – new user welcome credits
        Earned = 1,          // FR3.2 – user provided a skill (on completion)
        Spent = 2,           // FR3.3 – credits deducted from hold on completion
        AdminAdjustment = 3, // FR11.3 – admin froze / adjusted credits
        Refund = 4,          // Credits returned after cancel/reject
        CreditHold = 5,      // Credits moved to escrow when session is created
        CreditRelease = 6    // Held credits released back (cancel/reject)
    }

    /// <summary>
    /// Tracks every credit movement for a user (FR3.1–FR3.5)
    /// </summary>
    public class CreditTransaction:BaseEntity
    {
  

       
        public int UserId { get; set; }

        public TransactionType Type { get; set; }


        public int Amount { get; set; }

        /// <summary>Running balance after this transaction</summary>
     
        public int BalanceAfter { get; set; }

        /// <summary>Optional reference to the session that triggered this transaction</summary>

        public int? SessionId { get; set; }

        public string? Description { get; set; }

      
        // Navigation properties
        public User User { get; set; } = null!;
        public Session? Session { get; set; }
    }

}
