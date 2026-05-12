using MediatR;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IIdentityService _identityService;
        private readonly ISkillLinkDbContext _context;

        public VerifyEmailCommandHandler(IIdentityService identityService, ISkillLinkDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            var (success, errors) = await _identityService.VerifyEmailAsync(user.IdentityId, request.Token);
            if (!success)
            {
                throw new InvalidOperationException($"Email verification failed: {string.Join(", ", errors)}");
            }

            user.IsEmailVerified = true;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
