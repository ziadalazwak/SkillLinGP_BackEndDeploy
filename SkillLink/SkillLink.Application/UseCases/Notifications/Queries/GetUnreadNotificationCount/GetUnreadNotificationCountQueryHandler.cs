using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Notifications.Queries.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUnreadNotificationCountQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted, cancellationToken);
        }
    }
}
