using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Login
{
    public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;
}
