using MediatR;
using SkillLink.Application.Common;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTrades
{
    public enum TradeRoleFilter
    {
        Offerer = 0,
        Receiver = 1,
        Both = 2
    }

    public record GetTradesQuery : IRequest<PaginatedList<SkillTradeSummaryDto>>
    {
        public int UserId { get; set; }
        public TradeRoleFilter Role { get; init; } = TradeRoleFilter.Both;
        public TradeStatus? Status { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
