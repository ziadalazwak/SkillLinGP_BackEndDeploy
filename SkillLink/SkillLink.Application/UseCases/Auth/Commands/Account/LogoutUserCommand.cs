using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public record LogoutUserCommand(string RefreshToken) : IRequest<bool>;
}
