using MediatR;

namespace SkillLink.Application.UseCases.Admin.Commands.UnbanUser
{
    public record UnbanUserCommand(int UserId) : IRequest<bool>;
}
