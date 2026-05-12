using MediatR;

namespace SkillLink.Application.UseCases.Skills.Commands.DeleteSkill
{
    public record DeleteSkillCommand(int Id, int ProviderId) : IRequest<bool>;
}
