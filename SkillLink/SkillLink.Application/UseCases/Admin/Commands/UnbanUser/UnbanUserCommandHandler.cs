using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Commands.UnbanUser
{
    public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public UnbanUserCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null) return false;

            user.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
