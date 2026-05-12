using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.Interfaces
{
    public interface IMessageDispatcher
    {
        Task NotifyNewMessageAsync(int receiverId, MessageDto message);

        /// <summary>
        /// Notifies the original sender that the receiver has read their messages.
        /// Triggers a "MessagesRead" SignalR event on the sender's connection.
        /// </summary>
        Task NotifyMessagesReadAsync(int senderId, int readByUserId);

        /// <summary>
        /// Pushes the current unread count for a specific conversation to a user.
        /// Triggers an "UpdateUnreadCount" SignalR event on the user's connection.
        /// </summary>
        Task NotifyUnreadCountAsync(int userId, int conversationUserId, int unreadCount);
    }
}
