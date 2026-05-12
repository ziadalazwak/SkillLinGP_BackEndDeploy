using MediatR;

namespace SkillLink.Application.UseCases.Notifications.Queries.GetUnreadNotificationCount
{
    public record GetUnreadNotificationCountQuery(int UserId) : IRequest<int>;
}
