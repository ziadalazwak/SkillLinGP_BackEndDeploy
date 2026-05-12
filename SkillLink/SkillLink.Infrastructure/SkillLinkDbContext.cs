using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Infrastructure
{
    public class SkillLinkDbContext : IdentityDbContext<ApplicationUser>,ISkillLinkDbContext

    {
        public SkillLinkDbContext(DbContextOptions<SkillLinkDbContext> options) : base(options)
        {
        }
        public DbSet<User> DomainUsers { get; set; } = null!;
        public DbSet<SkillCategory> SkillCategories { get; set; } = null!;
        public DbSet<UserSkill> UserSkills { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<SkillTrade> SkillTrades { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<CreditTransaction> CreditTransactions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User ──────────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.HasIndex(u => u.PhoneNumber).IsUnique();
                e.Property(u => u.CreditBalance).HasPrecision(18, 2);
                e.Property(u => u.HeldCreditBalance).HasPrecision(18, 2);
            });

            // ── Session ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Session>(e =>
            {
                e.Property(s => s.CreditCost).HasPrecision(18, 2);

                e.HasOne(s => s.Provider)
                 .WithMany(u => u.SessionsAsProvider)
                 .HasForeignKey(s => s.ProviderId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(s => s.Requester)
                 .WithMany(u => u.SessionsAsRequester)
                 .HasForeignKey(s => s.RequesterId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── SkillTrade ────────────────────────────────────────────────────────
            modelBuilder.Entity<SkillTrade>(e =>
            {
                e.HasOne(t => t.Offerer)
                 .WithMany(u => u.TradesAsOfferer)
                 .HasForeignKey(t => t.OffererId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.Receiver)
                 .WithMany(u => u.TradesAsReceiver)
                 .HasForeignKey(t => t.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.OfferedSkill)
                 .WithMany(s => s.TradesOffered)
                 .HasForeignKey(t => t.OfferedSkillId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.RequestedSkill)
                 .WithMany(s => s.TradesRequested)
                 .HasForeignKey(t => t.RequestedSkillId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Review ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Review>(e =>
            {
                e.HasOne(r => r.Reviewer)
                 .WithMany(u => u.ReviewsGiven)
                 .HasForeignKey(r => r.ReviewerId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Reviewee)
                 .WithMany(u => u.ReviewsReceived)
                 .HasForeignKey(r => r.RevieweeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Message ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Message>(e =>
            {
                e.HasOne(m => m.Sender)
                 .WithMany(u => u.MessagesSent)
                 .HasForeignKey(m => m.SenderId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.Receiver)
                 .WithMany(u => u.MessagesReceived)
                 .HasForeignKey(m => m.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Skill>(e =>
            {
                // Removed direct User/Provider mapping because it's now handled via UserSkill bridge
            });

            // ── UserSkill ─────────────────────────────────────────────────────────
            modelBuilder.Entity<UserSkill>(e =>
            {
                // Unique constraint so a user cannot offer the same exact skill twice
                e.HasIndex(us => new { us.UserId, us.SkillId }).IsUnique();

                e.HasOne(us => us.User)
                 .WithMany(u => u.UserSkills)
                 .HasForeignKey(us => us.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(us => us.Skill)
                 .WithMany(s => s.UserSkills)
                 .HasForeignKey(us => us.SkillId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── UserSkill — composite uniqueness ──────────────────────────────────


            // ── CreditTransaction ─────────────────────────────────────────────────
            modelBuilder.Entity<CreditTransaction>(e =>
            {
                e.Property(ct => ct.Amount).HasPrecision(18, 2);
                e.Property(ct => ct.BalanceAfter).HasPrecision(18, 2);
            });

            // ── RefreshToken ──────────────────────────────────────────────────────
            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.HasIndex(rt => rt.Token).IsUnique();
                
                e.HasOne(rt => rt.User)
                 .WithMany(u => u.RefreshTokens)
                 .HasForeignKey(rt => rt.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    } }
