using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Notifications.DTOs;

namespace SkillLink.Application.UseCases.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUserNotificationsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == request.UserId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NotificationDto(
                    n.Id,
                    n.Type,
                    n.Title,
                    n.Body,
                    n.ActionUrl,
                    n.IsRead,
                    n.CreatedAt,
                    n.ReadAt
                ))
                .ToListAsync(cancellationToken);

            return notifications;
        }
    }
}
