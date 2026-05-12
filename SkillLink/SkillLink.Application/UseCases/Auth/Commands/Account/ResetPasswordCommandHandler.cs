using MediatR;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var (success, errors) = await _identityService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (!success)
            {
                throw new InvalidOperationException($"Password reset failed: {string.Join(", ", errors)}");
            }

            return true;
        }
    }
}
