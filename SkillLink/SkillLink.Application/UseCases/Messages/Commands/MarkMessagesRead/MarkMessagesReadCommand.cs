using MediatR;

namespace SkillLink.Application.UseCases.Messages.Commands.MarkMessagesRead
{
    /// <summary>
    /// Marks all unread messages sent by <see cref="OtherUserId"/> to <see cref="CurrentUserId"/> as read,
    /// then fires SignalR events so both parties' UIs update instantly.
    /// </summary>
    public record MarkMessagesReadCommand(int CurrentUserId, int OtherUserId) : IRequest;
}
