using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Notifications.DTOs
{
    public record NotificationDto(
        int Id,
        NotificationType Type,
        string Title,
        string? Body,
        string? ActionUrl,
        bool IsRead,
        DateTime CreatedAt,
        DateTime? ReadAt
    );
}
