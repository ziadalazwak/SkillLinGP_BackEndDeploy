using MediatR;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public record GetUserSkillByIdQuery(int UserId, int SkillId) : IRequest<UserSkillDetailDto?>;
}
