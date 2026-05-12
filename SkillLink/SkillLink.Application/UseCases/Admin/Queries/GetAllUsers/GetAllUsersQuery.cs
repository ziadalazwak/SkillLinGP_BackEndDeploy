using MediatR;
using SkillLink.Application.Common;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<PaginatedList<AdminUserDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
