using MediatR;

namespace SkillLink.Application.UseCases.Notifications.Commands.MarkNotificationRead
{
    public record MarkNotificationReadCommand(int NotificationId, int UserId) : IRequest<bool>;
}
