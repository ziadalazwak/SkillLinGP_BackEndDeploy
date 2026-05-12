using Microsoft.AspNetCore.SignalR;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Notifications.DTOs;
using SkillLink.API.Hubs;
using SkillLink.Domain.Models;

namespace SkillLink.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ISkillLinkDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(int userId, NotificationType type, string title, string? body = null, string? actionUrl = null, CancellationToken cancellationToken = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Body = body,
                ActionUrl = actionUrl,
                IsRead = false
            };

            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new NotificationDto(
                notification.Id,
                notification.Type,
                notification.Title,
                notification.Body,
                notification.ActionUrl,
                notification.IsRead,
                notification.CreatedAt,
                notification.ReadAt
            );

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", dto, cancellationToken);
        }
    }
}
