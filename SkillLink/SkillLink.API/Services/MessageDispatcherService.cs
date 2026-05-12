using Microsoft.AspNetCore.SignalR;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Messages.DTOs;
using SkillLink.API.Hubs;

namespace SkillLink.API.Services
{
    public class MessageDispatcherService : IMessageDispatcher
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageDispatcherService(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewMessageAsync(int receiverId, MessageDto message)
        {
            // Pushes target specifically to the User based on their ClaimTypes.NameIdentifier
            await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
        }

        /// <summary>
        /// Tells the original sender that <paramref name="readByUserId"/> has read their messages.
        /// The sender's UI uses this to flip message ticks to double-read receipts.
        /// </summary>
        public async Task NotifyMessagesReadAsync(int senderId, int readByUserId)
        {
            await _hubContext.Clients.User(senderId.ToString())
                .SendAsync("MessagesRead", new { readByUserId });
        }

        /// <summary>
        /// Tells <paramref name="userId"/> what their current unread count is for the
        /// conversation with <paramref name="conversationUserId"/>.
        /// Called with unreadCount = 0 immediately after the user reads the thread.
        /// </summary>
        public async Task NotifyUnreadCountAsync(int userId, int conversationUserId, int unreadCount)
        {
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("UpdateUnreadCount", new { conversationUserId, unreadCount });
        }
    }
}
