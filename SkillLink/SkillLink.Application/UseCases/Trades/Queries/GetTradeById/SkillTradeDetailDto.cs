using SkillLink.Domain.Models;
using System;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTradeById
{
    public record SkillTradeDetailDto(
        int Id,
        int OffererId,
        string OffererName,
        string? OffererProfilePicture,
        int OfferedSkillId,
        string OfferedSkillTitle,
        string? OfferedSkillDescription,
        int ReceiverId,
        string ReceiverName,
        string? ReceiverProfilePicture,
        int RequestedSkillId,
        string RequestedSkillTitle,
        string? RequestedSkillDescription,
        TradeStatus Status,
        string? Message,
        string? MeetingLink,
        DateTime CreatedAt,
        DateTime? CompletedAt
    );
}
