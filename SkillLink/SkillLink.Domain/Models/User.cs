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
    public class User:BaseEntity
    {
        public string? FullName { get; set; }

        
        public string Email { get; set; } = string.Empty;

       
        public string? PhoneNumber { get; set; }

      

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;  

        public string IdentityId { get; set; } = string.Empty; // Link to ASP.NET Identity user 


        public int CreditBalance { get; private set; }

        /// <summary>Credits currently held (escrowed) for pending/accepted sessions.</summary>
        public decimal HeldCreditBalance { get; private set; }
     
        public ICollection<UserSkill> UserSkills { get; set; } = [];
        public ICollection<Session> SessionsAsProvider { get; set; } = [];
        public ICollection<Session> SessionsAsRequester { get; set; } = [];
        public ICollection<Review> ReviewsGiven { get; set; } = [];
        public ICollection<Review> ReviewsReceived { get; set; } = [];
        public ICollection<Message> MessagesSent { get; set; } = [];
        public ICollection<Message> MessagesReceived { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
        public ICollection<CreditTransaction> CreditTransactions { get; set; } = [];
        public ICollection<SkillTrade> TradesAsOfferer { get; set; } = [];
        public ICollection<SkillTrade> TradesAsReceiver { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public void AdjustCreditBalance(int amount)
        {
            CreditBalance += amount;
            if (CreditBalance < 0)
            {
                throw new InvalidOperationException("Credit balance cannot be negative.");
            }
        }

        /// <summary>Reserve (hold) credits from available balance into escrow.</summary>
        public void HoldCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Hold amount must be positive.");
            if (CreditBalance < amount)
                throw new InvalidOperationException("Insufficient credit balance to hold.");
            CreditBalance -= amount;
            HeldCreditBalance += amount;
        }

        /// <summary>Release held credits back to available balance (refund on cancel/reject).</summary>
        public void ReleaseHeldCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Release amount must be positive.");
            if (HeldCreditBalance < amount)
                throw new InvalidOperationException("Held balance is insufficient.");
            HeldCreditBalance -= amount;
            CreditBalance += amount;
        }

        /// <summary>Settle held credits — remove from escrow (payment sent to provider separately).</summary>
        public void SettleHeldCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Settle amount must be positive.");
            if (HeldCreditBalance < amount)
                throw new InvalidOperationException("Held balance is insufficient.");
            HeldCreditBalance -= amount;
        }
    }
}
