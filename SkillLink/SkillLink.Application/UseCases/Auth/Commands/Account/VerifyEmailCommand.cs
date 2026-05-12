using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public record VerifyEmailCommand(string Email, string Token) : IRequest<bool>;
}
