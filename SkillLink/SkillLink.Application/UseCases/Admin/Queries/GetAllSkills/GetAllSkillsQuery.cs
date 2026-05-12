using MediatR;
using SkillLink.Application.Common;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSkills
{
    public class GetAllSkillsQuery : IRequest<PaginatedList<AdminSkillDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
