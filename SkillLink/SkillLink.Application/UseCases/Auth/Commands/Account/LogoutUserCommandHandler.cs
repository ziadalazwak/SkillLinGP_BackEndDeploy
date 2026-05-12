using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;

        public LogoutUserCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (token != null)
            {
                token.Revoked = DateTime.UtcNow;
                token.ReasonRevoked = "User logged out";
                await _context.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
