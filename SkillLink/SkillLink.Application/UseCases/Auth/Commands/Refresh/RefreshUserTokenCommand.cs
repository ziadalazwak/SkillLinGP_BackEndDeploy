using MediatR;
using SkillLink.Application.UseCases.Auth.Commands.Login;

namespace SkillLink.Application.UseCases.Auth.Commands.Refresh
{
    public record RefreshUserTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
}
