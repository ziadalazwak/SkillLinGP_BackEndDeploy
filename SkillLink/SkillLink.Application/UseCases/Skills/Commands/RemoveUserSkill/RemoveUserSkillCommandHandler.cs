using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Commands.RemoveUserSkill
{
    public class RemoveUserSkillCommandHandler : IRequestHandler<RemoveUserSkillCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public RemoveUserSkillCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveUserSkillCommand request, CancellationToken cancellationToken)
        {
            var userSkill = await _context.UserSkills
                .FirstOrDefaultAsync(us => us.UserId == request.UserId && us.SkillId == request.SkillId && !us.IsDeleted, cancellationToken);

            if (userSkill == null)
            {
                return false;
            }

            userSkill.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
