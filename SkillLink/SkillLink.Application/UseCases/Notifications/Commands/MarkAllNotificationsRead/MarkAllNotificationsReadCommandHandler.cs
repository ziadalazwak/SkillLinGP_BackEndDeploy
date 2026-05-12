using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Notifications.Commands.MarkAllNotificationsRead
{
    public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, int>
    {
        private readonly ISkillLinkDbContext _context;

        public MarkAllNotificationsReadCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted)
                .ToListAsync(cancellationToken);

            if (unreadNotifications.Count == 0) return 0;

            var now = DateTime.UtcNow;
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                notification.UpdatedAt = now;
            }

            _context.Notifications.UpdateRange(unreadNotifications);
            await _context.SaveChangesAsync(cancellationToken);

            return unreadNotifications.Count;
        }
    }
}
