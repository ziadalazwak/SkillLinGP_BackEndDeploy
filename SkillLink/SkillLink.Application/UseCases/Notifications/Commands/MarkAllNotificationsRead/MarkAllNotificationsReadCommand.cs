using MediatR;

namespace SkillLink.Application.UseCases.Notifications.Commands.MarkAllNotificationsRead
{
    public record MarkAllNotificationsReadCommand(int UserId) : IRequest<int>;
}
