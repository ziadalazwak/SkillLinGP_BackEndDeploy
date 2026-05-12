using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Commands.BanUser
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public BanUserCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null) return false;

            user.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
