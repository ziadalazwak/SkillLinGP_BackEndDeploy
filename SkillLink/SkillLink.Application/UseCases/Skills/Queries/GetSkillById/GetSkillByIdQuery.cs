using MediatR;

namespace SkillLink.Application.UseCases.Skills.Queries.GetSkillById
{
    public record GetSkillByIdQuery(int Id) : IRequest<SkillDetailDto?>;
}
