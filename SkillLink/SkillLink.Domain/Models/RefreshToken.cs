using System;
using System.ComponentModel.DataAnnotations;

namespace SkillLink.Domain.Models
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        
        [MaxLength(200)]
        public string Token { get; set; } = string.Empty;
        
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        public DateTime? Revoked { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }
        
        // Active if it hasn't been revoked and hasn't expired.
        public bool IsActive => Revoked == null && !IsExpired;

        // Navigation property
        public User User { get; set; } = null!;
    }
}
