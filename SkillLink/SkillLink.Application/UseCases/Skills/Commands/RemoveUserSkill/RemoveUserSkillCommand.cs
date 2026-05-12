using MediatR;

namespace SkillLink.Application.UseCases.Skills.Commands.RemoveUserSkill
{
    public record RemoveUserSkillCommand(int UserId, int SkillId) : IRequest<bool>;
}
