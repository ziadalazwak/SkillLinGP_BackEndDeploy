using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public class GetUserSkillByIdQueryHandler : IRequestHandler<GetUserSkillByIdQuery, UserSkillDetailDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUserSkillByIdQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<UserSkillDetailDto?> Handle(GetUserSkillByIdQuery request, CancellationToken cancellationToken)
        {
            var raw = await _context.UserSkills
                .Include(us => us.Skill)
                    .ThenInclude(s => s.Category)
                .AsNoTracking()
                .Where(us => us.UserId  == request.UserId
                          && us.SkillId == request.SkillId
                          && !us.IsDeleted
                          && us.Skill.IsActive
                          && !us.Skill.IsDeleted)
                .Select(us => new
                {
                    us.SkillId,
                    us.Skill.Title,
                    us.Skill.Description,
                    CategoryName = us.Skill.Category.Name,
                    us.Skill.ImageUrl,
                    us.Level,
                    us.IsOffering,
                    us.ExchangeMode,
                    us.DurationMinutes,
                    us.DeliveryMode,
                    us.Notes
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (raw == null) return null;

            var creditCost = raw.DurationMinutes.HasValue
                ? (int)Math.Round(raw.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero)
                : (int?)null;

            return new UserSkillDetailDto(
                raw.SkillId,
                raw.Title,
                raw.Description,
                raw.CategoryName,
                raw.ImageUrl,
                raw.Level,
                raw.IsOffering,
                raw.ExchangeMode,
                raw.DurationMinutes,
                creditCost,
                raw.DeliveryMode,
                raw.Notes
            );
        }
    }
}
