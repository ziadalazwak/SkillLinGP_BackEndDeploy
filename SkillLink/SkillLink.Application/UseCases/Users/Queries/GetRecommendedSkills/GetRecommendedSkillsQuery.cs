using MediatR;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetRecommendedSkills
{
    public record GetRecommendedSkillsQuery(int UserId, int TopN = 5) : IRequest<IEnumerable<RecommendedSkillDto>>;
}
