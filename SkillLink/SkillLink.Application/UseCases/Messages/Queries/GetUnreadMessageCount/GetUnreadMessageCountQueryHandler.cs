using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Messages.Queries.GetUnreadMessageCount
{
    public class GetUnreadMessageCountQueryHandler : IRequestHandler<GetUnreadMessageCountQuery, int>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUnreadMessageCountQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetUnreadMessageCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == request.UserId && !m.IsRead && !m.IsDeleted, cancellationToken);
        }
    }
}
