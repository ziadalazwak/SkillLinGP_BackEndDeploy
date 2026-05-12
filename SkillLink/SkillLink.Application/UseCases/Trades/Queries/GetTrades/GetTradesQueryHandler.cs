using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;
using System.Linq;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTrades
{
    public class GetTradesQueryHandler : IRequestHandler<GetTradesQuery, PaginatedList<SkillTradeSummaryDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetTradesQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<SkillTradeSummaryDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.SkillTrades
                .Include(t => t.Offerer)
                .Include(t => t.Receiver)
                .Include(t => t.OfferedSkill)
                .Include(t => t.RequestedSkill)
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .AsQueryable();

            if (request.Role == TradeRoleFilter.Offerer)
            {
                query = query.Where(t => t.OffererId == request.UserId);
            }
            else if (request.Role == TradeRoleFilter.Receiver)
            {
                query = query.Where(t => t.ReceiverId == request.UserId);
            }
            else
            {
                query = query.Where(t => t.OffererId == request.UserId || t.ReceiverId == request.UserId);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new SkillTradeSummaryDto(
                    t.Id,
                    t.OffererId,
                    t.Offerer.FullName ?? "Unknown",
                    t.OfferedSkillId,
                    t.OfferedSkill.Title,
                    t.ReceiverId,
                    t.Receiver.FullName ?? "Unknown",
                    t.RequestedSkillId,
                    t.RequestedSkill.Title,
                    t.Status,
                    t.MeetingLink == "" ? null : t.MeetingLink,
                    t.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedList<SkillTradeSummaryDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
