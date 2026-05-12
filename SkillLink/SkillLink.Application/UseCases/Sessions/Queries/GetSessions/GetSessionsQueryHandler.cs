using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;
using System.Linq;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessions
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, PaginatedList<SessionSummaryDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetSessionsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<SessionSummaryDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Sessions
                .Include(s => s.Skill)
                .Include(s => s.Provider)
                .Include(s => s.Requester)
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            // Filter by Role
            if (request.Role == SessionRoleFilter.Requester)
            {
                query = query.Where(s => s.RequesterId == request.UserId);
            }
            else if (request.Role == SessionRoleFilter.Provider)
            {
                query = query.Where(s => s.ProviderId == request.UserId);
            }
            else
            {
                query = query.Where(s => s.RequesterId == request.UserId || s.ProviderId == request.UserId);
            }

            // Filter by Status
            if (request.Status.HasValue)
            {
                query = query.Where(s => s.Status == request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SessionSummaryDto(
                    s.Id,
                    s.SkillId,
                    s.Skill.Title,
                    s.ProviderId,
                    s.Provider.FullName ?? "Unknown",
                    s.RequesterId,
                    s.Requester.FullName ?? "Unknown",
                    s.Status,
                    s.ScheduledAt,
                    s.CreditCost,
                    s.MeetingLink,
                    s.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedList<SessionSummaryDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
