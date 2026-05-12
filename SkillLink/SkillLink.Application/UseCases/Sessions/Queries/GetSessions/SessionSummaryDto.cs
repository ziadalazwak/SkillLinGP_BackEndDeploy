using SkillLink.Domain.Models;
using System;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessions
{
    public record SessionSummaryDto(
        int Id,
        int SkillId,
        string SkillTitle,
        int ProviderId,
        string ProviderName,
        int RequesterId,
        string RequesterName,
        SessionStatus Status,
        DateTime? ScheduledAt,
        int? CreditCost,
        string? MeetingLink,
        DateTime CreatedAt
    );
}
