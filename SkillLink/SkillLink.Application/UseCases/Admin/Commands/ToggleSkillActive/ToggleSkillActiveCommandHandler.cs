using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Commands.ToggleSkillActive
{
    public class ToggleSkillActiveCommandHandler : IRequestHandler<ToggleSkillActiveCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public ToggleSkillActiveCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ToggleSkillActiveCommand request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.SkillId && !s.IsDeleted, cancellationToken);

            if (skill is null)
                return false;

            skill.IsActive = !skill.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
