using MediatR;

namespace SkillLink.Application.UseCases.Skills.Queries.GetUserSkills
{
    public record GetUserSkillsQuery(int ProviderId) : IRequest<List<UserSkillDto>>;
}
