using MediatR;

namespace SkillLink.Application.UseCases.Admin.Commands.DeleteUser
{
    public record DeleteUserCommand(int UserId) : IRequest<bool>;
}
