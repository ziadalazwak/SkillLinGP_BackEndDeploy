using Microsoft.EntityFrameworkCore;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.Interfaces
{
    public interface ISkillLinkDbContext
    {
       
        DbSet<SkillCategory> SkillCategories { get; }
        DbSet<UserSkill> UserSkills { get; }
        DbSet<Skill> Skills { get; }
        DbSet<Session> Sessions { get; }
        DbSet<SkillTrade> SkillTrades { get; }
        DbSet<User> DomainUsers { get; }
        DbSet<CreditTransaction> CreditTransactions { get; }
        DbSet<Review> Reviews { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Message> Messages { get; }
        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
