using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Messages.Commands.MarkMessagesRead
{
    public class MarkMessagesReadCommandHandler : IRequestHandler<MarkMessagesReadCommand>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IMessageDispatcher _messageDispatcher;

        public MarkMessagesReadCommandHandler(ISkillLinkDbContext context, IMessageDispatcher messageDispatcher)
        {
            _context = context;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Handle(MarkMessagesReadCommand request, CancellationToken cancellationToken)
        {
            var unreadMessages = await _context.Messages
                .Where(m => !m.IsDeleted &&
                            m.SenderId == request.OtherUserId &&
                            m.ReceiverId == request.CurrentUserId &&
                            !m.IsRead)
                .ToListAsync(cancellationToken);

            if (unreadMessages.Count == 0)
                return;

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Notify the original sender that their messages were read (enables double-tick receipts)
            await _messageDispatcher.NotifyMessagesReadAsync(
                senderId: request.OtherUserId,
                readByUserId: request.CurrentUserId);

            // Tell the current user their unread count for this conversation is now 0
            await _messageDispatcher.NotifyUnreadCountAsync(
                userId: request.CurrentUserId,
                conversationUserId: request.OtherUserId,
                unreadCount: 0);
        }
    }
}
