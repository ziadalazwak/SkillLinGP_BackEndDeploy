using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>;
}
