using SkillLink.Domain.Models;
using System;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessionById
{
    public record SessionDetailDto(
        int Id,
        int SkillId,
        string SkillTitle,
        string? SkillDescription,
        int ProviderId,
        string ProviderName,
        string? ProviderProfilePicture,
        int RequesterId,
        string RequesterName,
        string? RequesterProfilePicture,
        SessionStatus Status,
        DateTime? ScheduledAt,
        int? DurationMinutes,
        int? CreditCost,
        string? MeetingLink,
        string? RequesterNote,
        string? ProviderNote,
        DateTime CreatedAt,
        DateTime? CompletedAt
    );
}
