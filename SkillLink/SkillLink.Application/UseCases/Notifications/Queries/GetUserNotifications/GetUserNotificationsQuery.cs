using MediatR;
using SkillLink.Application.UseCases.Notifications.DTOs;

namespace SkillLink.Application.UseCases.Notifications.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery(int UserId, int Page = 1, int PageSize = 20) : IRequest<List<NotificationDto>>;
}
