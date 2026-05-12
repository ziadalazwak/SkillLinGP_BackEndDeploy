using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public class GetUserSkillsQueryHandler : IRequestHandler<GetUserSkillsQuery, List<UserSkillDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUserSkillsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserSkillDto>> Handle(GetUserSkillsQuery request, CancellationToken cancellationToken)
        {
            var raw = await _context.UserSkills
                .Include(us => us.Skill)
                .AsNoTracking()
                .Where(us => us.UserId == request.ProviderId && us.Skill.IsActive && !us.IsDeleted && !us.Skill.IsDeleted)
                .Select(us => new
                {
                    us.SkillId,
                    us.Skill.Title,
                    us.Level,
                    us.IsOffering,
                    us.ExchangeMode,
                    us.DurationMinutes,
                    DeliveryMode = us.DeliveryMode ?? ""
                })
                .ToListAsync(cancellationToken);

            var skills = raw.Select(us => new UserSkillDto(
                    us.SkillId,
                    us.Title,
                    us.Level,
                    us.IsOffering,
                    us.ExchangeMode,
                    us.DurationMinutes,
                    us.DeliveryMode,
                    us.DurationMinutes.HasValue
                        ? (int)Math.Round(us.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero)
                        : (int?)null
                ))
                .ToList();

            return skills;
        }
    }
}
