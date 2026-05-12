using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}
