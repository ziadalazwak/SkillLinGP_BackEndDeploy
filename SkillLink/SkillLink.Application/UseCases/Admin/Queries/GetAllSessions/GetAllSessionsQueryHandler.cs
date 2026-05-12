using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSessions
{
    public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, PaginatedList<AdminSessionDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetAllSessionsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<AdminSessionDto>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Sessions
                .Include(s => s.Skill)
                .Include(s => s.Provider)
                .Include(s => s.Requester)
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(s => s.Status == request.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new AdminSessionDto
                {
                    Id          = s.Id,
                    SkillId     = s.SkillId,
                    SkillTitle  = s.Skill.Title,
                    Mentor      = new AdminSessionUserDto
                    {
                        Id       = s.Provider.Id,
                        FullName = s.Provider.FullName,
                        Email    = s.Provider.Email
                    },
                    Learner     = new AdminSessionUserDto
                    {
                        Id       = s.Requester.Id,
                        FullName = s.Requester.FullName,
                        Email    = s.Requester.Email
                    },
                    Status      = s.Status,
                    ScheduledAt = s.ScheduledAt,
                    CreditCost  = s.CreditCost,
                    MeetingLink = s.MeetingLink,
                    CreatedAt   = s.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<AdminSessionDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
