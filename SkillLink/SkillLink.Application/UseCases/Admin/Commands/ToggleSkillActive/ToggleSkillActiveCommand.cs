using MediatR;

namespace SkillLink.Application.UseCases.Admin.Commands.ToggleSkillActive
{
    public record ToggleSkillActiveCommand(int SkillId) : IRequest<bool>;
}
