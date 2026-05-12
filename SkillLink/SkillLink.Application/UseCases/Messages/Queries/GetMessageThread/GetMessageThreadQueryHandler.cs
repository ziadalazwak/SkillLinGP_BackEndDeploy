using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Queries.GetMessageThread
{
    public class GetMessageThreadQueryHandler : IRequestHandler<GetMessageThreadQuery, PaginatedList<MessageDto>>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IMessageDispatcher _messageDispatcher;

        public GetMessageThreadQueryHandler(ISkillLinkDbContext context, IMessageDispatcher messageDispatcher)
        {
            _context = context;
            _messageDispatcher = messageDispatcher;
        }

        public async Task<PaginatedList<MessageDto>> Handle(GetMessageThreadQuery request, CancellationToken cancellationToken)
        {
            // Verify they have an accepted/completed session or trade to unlock messaging
            bool hasSession = await _context.Sessions.AnyAsync(s => 
                !s.IsDeleted &&
                (s.Status == Domain.Models.SessionStatus.Accepted || s.Status == Domain.Models.SessionStatus.Completed) &&
                ((s.RequesterId == request.CurrentUserId && s.ProviderId == request.TargetUserId) || 
                 (s.RequesterId == request.TargetUserId && s.ProviderId == request.CurrentUserId)), cancellationToken);

            bool hasTrade = await _context.SkillTrades.AnyAsync(t => 
                !t.IsDeleted &&
                (t.Status == Domain.Models.TradeStatus.Accepted || t.Status == Domain.Models.TradeStatus.Completed) &&
                ((t.OffererId == request.CurrentUserId && t.ReceiverId == request.TargetUserId) || 
                 (t.OffererId == request.TargetUserId && t.ReceiverId == request.CurrentUserId)), cancellationToken);

            if (!hasSession && !hasTrade)
            {
                throw new UnauthorizedAccessException("You must have an accepted session or skill trade to message this user.");
            }

            var query = _context.Messages
                .Where(m => !m.IsDeleted &&
                    ((m.SenderId == request.CurrentUserId && m.ReceiverId == request.TargetUserId) ||
                     (m.SenderId == request.TargetUserId && m.ReceiverId == request.CurrentUserId)))
                .OrderByDescending(m => m.SentAt);

            var count = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);

            var unreadMessages = items
                .Where(m => m.ReceiverId == request.CurrentUserId && !m.IsRead)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                    msg.ReadAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync(cancellationToken);

                // Notify the original sender that their messages were read (enables double-tick receipts)
                await _messageDispatcher.NotifyMessagesReadAsync(
                    senderId: request.TargetUserId,
                    readByUserId: request.CurrentUserId);

                // Tell the current user their unread count for this conversation is now 0
                await _messageDispatcher.NotifyUnreadCountAsync(
                    userId: request.CurrentUserId,
                    conversationUserId: request.TargetUserId,
                    unreadCount: 0);
            }

            var dtos = items.Select(m => new MessageDto(
                m.Id,
                m.SenderId,
                m.ReceiverId,
                m.SessionId,
                m.Content,
                m.AttachmentUrl,
                m.IsRead,
                m.SentAt,
                m.ReadAt
            )).Reverse().ToList(); // Flip back descending so array is chronological

            return new PaginatedList<MessageDto>(dtos, count, request.Page, request.PageSize);
        }
    }
}
