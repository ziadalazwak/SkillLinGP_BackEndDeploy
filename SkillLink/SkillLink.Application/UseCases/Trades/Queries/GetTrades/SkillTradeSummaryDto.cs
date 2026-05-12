using SkillLink.Domain.Models;
using System;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTrades
{
    public record SkillTradeSummaryDto(
        int Id,
        int OffererId,
        string OffererName,
        int OfferedSkillId,
        string OfferedSkillTitle,
        int ReceiverId,
        string ReceiverName,
        int RequestedSkillId,
        string RequestedSkillTitle,
        TradeStatus Status,
        string? MeetingLink,
        DateTime CreatedAt
    );
}
