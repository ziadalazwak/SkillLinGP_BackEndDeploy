using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Queries.GetConversations
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, IEnumerable<ConversationSummaryDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetConversationsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConversationSummaryDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            // We group messages by the other participant's ID
            // For production apps with millions of messages, this should be done via a View or raw SQL, but in-memory grouping is fine for this scale
            var messages = await _context.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == request.UserId || m.ReceiverId == request.UserId) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            var conversations = new List<ConversationSummaryDto>();
            var groupedByCounterparty = messages.GroupBy(m => m.SenderId == request.UserId ? m.ReceiverId : m.SenderId);

            foreach (var group in groupedByCounterparty)
            {
                var targetId = group.Key;
                var latestMessage = group.OrderByDescending(m => m.SentAt).First();
                
                var targetUser = latestMessage.SenderId == request.UserId ? latestMessage.Receiver : latestMessage.Sender;
                var unreadCount = group.Count(m => m.ReceiverId == request.UserId && !m.IsRead);

                conversations.Add(new ConversationSummaryDto(
                    targetUser.Id,
                    targetUser.FullName ?? "Unknown",
                    targetUser.ProfilePictureUrl,
                    latestMessage.AttachmentUrl != null ? "📎 Attachment" : latestMessage.Content,
                    latestMessage.SentAt,
                    unreadCount
                ));
            }

            return conversations.OrderByDescending(c => c.LastMessageDate);
        }
    }
}
