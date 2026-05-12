using MediatR;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public ForgotPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var (success, token) = await _identityService.GeneratePasswordResetTokenAsync(request.Email);
            if (!success)
            {
                // To prevent enumeration, we always return true even if user not found for forgot password
                return true;
            }

            // TODO: Integrate with an Email Service to send the token
            // For now, we simulate success
            Console.WriteLine($"Password Reset Token for {request.Email}: {token}");

            return true;
        }
    }
}
