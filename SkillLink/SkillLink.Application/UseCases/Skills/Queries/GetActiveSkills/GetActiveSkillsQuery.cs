using MediatR;
using System.Collections.Generic;

namespace SkillLink.Application.UseCases.Skills.Queries.GetActiveSkills
{
    public record GetActiveSkillsQuery() : IRequest<IEnumerable<ActiveSkillDto>>;
}
