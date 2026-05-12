using MediatR;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Auth.Commands.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly ISkillLinkDbContext _context;

        public LoginUserCommandHandler(IIdentityService identityService, IJwtTokenGenerator jwtGenerator, ISkillLinkDbContext context)
        {
            _identityService = identityService;
            _jwtGenerator = jwtGenerator;
            _context = context;
        }

        public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (user.IsDeleted)
            {
                throw new UnauthorizedAccessException("Account does not exist.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is deactivated.");
            }

            if (!user.IsEmailVerified)
            {
                throw new UnauthorizedAccessException("EMAIL_NOT_VERIFIED");
            }

            var passwordValid = await _identityService.CheckPasswordAsync(user.IdentityId, request.Password);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var roles = new List<string> { "User" };
            if (user.IsAdmin) roles.Add("Admin");

            var jwtToken = _jwtGenerator.GenerateToken(user, roles);
            var refreshTokenString = _jwtGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponse(jwtToken, refreshTokenString, refreshToken.Expires, user.Id);
        }
    }
}
