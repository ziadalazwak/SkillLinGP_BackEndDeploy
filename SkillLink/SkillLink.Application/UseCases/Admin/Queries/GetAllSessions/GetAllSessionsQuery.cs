using MediatR;
using SkillLink.Application.Common;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSessions
{
    public class GetAllSessionsQuery : IRequest<PaginatedList<AdminSessionDto>>
    {
        public SessionStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
