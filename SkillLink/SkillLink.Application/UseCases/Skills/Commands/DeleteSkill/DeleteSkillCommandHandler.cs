using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Commands.DeleteSkill
{
    public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public DeleteSkillCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (skill == null)
            {
                return false;
            }

            skill.IsDeleted = true;
            skill.IsActive  = false;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
