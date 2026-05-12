using MediatR;

namespace SkillLink.Application.UseCases.Admin.Commands.BanUser
{
    public record BanUserCommand(int UserId) : IRequest<bool>;
}
