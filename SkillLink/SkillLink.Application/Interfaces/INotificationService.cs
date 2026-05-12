using SkillLink.Domain.Models;

namespace SkillLink.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, NotificationType type, string title, string? body = null, string? actionUrl = null, CancellationToken cancellationToken = default);
    }
}
