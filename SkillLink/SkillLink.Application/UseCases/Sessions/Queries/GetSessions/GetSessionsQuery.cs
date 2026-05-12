using MediatR;
using SkillLink.Application.Common;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessions
{
    public enum SessionRoleFilter
    {
        Requester = 0,
        Provider = 1,
        Both = 2
    }

    public record GetSessionsQuery : IRequest<PaginatedList<SessionSummaryDto>>
    {
        public int UserId { get; set; }
        public SessionRoleFilter Role { get; init; } = SessionRoleFilter.Both;
        public SessionStatus? Status { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
