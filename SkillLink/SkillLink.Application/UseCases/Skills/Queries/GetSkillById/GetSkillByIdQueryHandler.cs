using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Queries.GetSkillById
{
    public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, SkillDetailDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetSkillByIdQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<SkillDetailDto?> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .Include(s => s.Category)
                .Include(s => s.UserSkills)
                    .ThenInclude(us => us.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive && !s.IsDeleted, cancellationToken);

            if (skill == null) return null;

            var providers = skill.UserSkills
                .Where(us => us.IsOffering && !us.IsDeleted && !us.User.IsDeleted)
                .Select(us => new ProviderOfferDto
                {
                    ProviderId       = us.UserId,
                    ProviderName     = us.User.FullName ?? "Unknown User",
                    ProfilePictureUrl = us.User.ProfilePictureUrl,
                    Level            = us.Level,
                    ExchangeMode     = us.ExchangeMode,
                    CreditCost       = us.DurationMinutes.HasValue ? (int)Math.Round(us.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero) : (int?)null,
                    DurationMinutes  = us.DurationMinutes,
                    DeliveryMode     = us.DeliveryMode,
                    Notes            = us.Notes
                })
                .ToList();

            return new SkillDetailDto(
                skill.Id,
                skill.Title,
                skill.Description,
                skill.Category.Name,
                skill.ImageUrl ?? "",
                skill.CreatedAt,
                providers
            );
        }
    }
}
