using MediatR;
using System.IO;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Stream? AttachmentStream { get; set; }
        public string? AttachmentFileName { get; set; }
    }
}
