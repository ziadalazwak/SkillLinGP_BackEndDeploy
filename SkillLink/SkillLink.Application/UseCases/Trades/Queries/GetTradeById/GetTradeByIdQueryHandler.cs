using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System.Security.Authentication;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTradeById
{
    public class GetTradeByIdQueryHandler : IRequestHandler<GetTradeByIdQuery, SkillTradeDetailDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetTradeByIdQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<SkillTradeDetailDto?> Handle(GetTradeByIdQuery request, CancellationToken cancellationToken)
        {
            var trade = await _context.SkillTrades
                .Include(t => t.Offerer)
                .Include(t => t.Receiver)
                .Include(t => t.OfferedSkill)
                .Include(t => t.RequestedSkill)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (trade == null) return null;

            if (trade.OffererId != request.UserId && trade.ReceiverId != request.UserId)
            {
                throw new AuthenticationException("You are not a participant in this trade.");
            }

            return new SkillTradeDetailDto(
                trade.Id,
                trade.OffererId,
                trade.Offerer?.FullName ?? "Unknown Offerer",
                trade.Offerer?.ProfilePictureUrl,
                trade.OfferedSkillId,
                trade.OfferedSkill?.Title ?? "Unknown Skill",
                trade.OfferedSkill?.Description,
                trade.ReceiverId,
                trade.Receiver?.FullName ?? "Unknown Receiver",
                trade.Receiver?.ProfilePictureUrl,
                trade.RequestedSkillId,
                trade.RequestedSkill?.Title ?? "Unknown Skill",
                trade.RequestedSkill?.Description,
                trade.Status,
                trade.Message,
                string.IsNullOrEmpty(trade.MeetingLink) ? null : trade.MeetingLink,
                trade.CreatedAt,
                trade.CompletedAt
            );
        }
    }
}
