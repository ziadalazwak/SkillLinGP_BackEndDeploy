using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Skills.Queries.GetActiveSkills
{
    public class GetActiveSkillsQueryHandler : IRequestHandler<GetActiveSkillsQuery, IEnumerable<ActiveSkillDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetActiveSkillsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActiveSkillDto>> Handle(GetActiveSkillsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Skills
                .AsNoTracking()
                .Include(s => s.Category)
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Title)
                .Select(s => new ActiveSkillDto(
                    s.Id,
                    s.Title,
                    s.Category.Name,
                    s.Description,
                    s.ImageUrl
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
