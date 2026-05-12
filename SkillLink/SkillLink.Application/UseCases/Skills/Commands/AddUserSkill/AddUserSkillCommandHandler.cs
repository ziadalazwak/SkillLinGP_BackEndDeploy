using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Skills.Commands.AddUserSkill
{
    public class AddUserSkillCommandHandler : IRequestHandler<AddUserSkillCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public AddUserSkillCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddUserSkillCommand request, CancellationToken cancellationToken)
        {
            // ── Validate skill exists ────────────────────────────────────────
            var skillExists = await _context.Skills
                .AnyAsync(s => s.Id == request.SkillId && s.IsActive && !s.IsDeleted, cancellationToken);
            if (!skillExists)
                throw new KeyNotFoundException("Skill not found or inactive.");

            // ── Check for duplicate ──────────────────────────────────────────
            var alreadyOffers = await _context.UserSkills
                .AnyAsync(us => us.UserId == request.UserId && us.SkillId == request.SkillId && !us.IsDeleted, cancellationToken);
            if (alreadyOffers)
                throw new InvalidOperationException("User already offers this skill.");

            // ── Duration validation (30–180 min) ─────────────────────────────
            if (request.DurationMinutes.HasValue)
            {
                if (request.DurationMinutes.Value < 30)
                    throw new InvalidOperationException("Minimum session duration is 30 minutes.");
                if (request.DurationMinutes.Value > 180)
                    throw new InvalidOperationException("Maximum session duration is 180 minutes (3 hours).");
            }

            var userSkill = new UserSkill
            {
                UserId          = request.UserId,
                SkillId         = request.SkillId,
                Level           = request.Level,
                ExchangeMode    = request.ExchangeMode,
                DurationMinutes = request.DurationMinutes,
                DeliveryMode    = request.DeliveryMode,
                Notes           = request.Notes,
                IsOffering      = true
            };

            await _context.UserSkills.AddAsync(userSkill, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
