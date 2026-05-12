using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Auth.Commands.Login;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Auth.Commands.Refresh
{
    public class RefreshUserTokenCommandHandler : IRequestHandler<RefreshUserTokenCommand, AuthResponse>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public RefreshUserTokenCommandHandler(ISkillLinkDbContext context, IJwtTokenGenerator jwtGenerator)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<AuthResponse> Handle(RefreshUserTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (storedToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (!storedToken.IsActive)
            {
                throw new UnauthorizedAccessException("Refresh token is expired or revoked.");
            }

            var user = storedToken.User;
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is deactivated.");
            }

            // Revoke current token
            storedToken.Revoked = DateTime.UtcNow;
            storedToken.ReasonRevoked = "Replaced by new token";

            // Generate new token pair
            var roles = new List<string> { "User" };
            if (user.IsAdmin) roles.Add("Admin");

            var newJwt = _jwtGenerator.GenerateToken(user, roles);
            var newRefreshTokenString = _jwtGenerator.GenerateRefreshToken();

            storedToken.ReplacedByToken = newRefreshTokenString;

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenString,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponse(newJwt, newRefreshTokenString, newRefreshToken.Expires, user.Id);
        }
    }
}
