using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationService _notificationService;

        public SendMessageCommandHandler(ISkillLinkDbContext context, IMessageDispatcher messageDispatcher, IFileStorageService fileStorageService, INotificationService notificationService)
        {
            _context = context;
            _messageDispatcher = messageDispatcher;
            _fileStorageService = fileStorageService;
            _notificationService = notificationService;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            bool hasSession = await _context.Sessions.AnyAsync(s => 
               
                ((s.RequesterId == request.SenderId && s.ProviderId == request.ReceiverId) || 
                 (s.RequesterId == request.ReceiverId && s.ProviderId == request.SenderId)), cancellationToken);

            bool hasTrade = await _context.SkillTrades.AnyAsync(t => 
                
                ((t.OffererId == request.SenderId && t.ReceiverId == request.ReceiverId) || 
                 (t.OffererId == request.ReceiverId && t.ReceiverId == request.SenderId)), cancellationToken);

            if (!hasSession && !hasTrade)
            {
                throw new UnauthorizedAccessException("You must have a session or skill trade to message this user.");
            }

            string? attachmentUrl = null;
            if (request.AttachmentStream != null && !string.IsNullOrEmpty(request.AttachmentFileName))
            {
                attachmentUrl = await _fileStorageService.SaveAsync(request.AttachmentStream, request.AttachmentFileName, "attachments", cancellationToken);
            }

            var message = new Domain.Models.Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                SessionId = request.SessionId,
                Content = request.Content,
                AttachmentUrl = attachmentUrl,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new MessageDto(
                message.Id,
                message.SenderId,
                message.ReceiverId,
                message.SessionId,
                message.Content,
                message.AttachmentUrl,
                message.IsRead,
                message.SentAt,
                message.ReadAt
            );

            // SignalR Trigger
            await _messageDispatcher.NotifyNewMessageAsync(message.ReceiverId, dto);

            await _notificationService.SendNotificationAsync(
                message.ReceiverId,
                Domain.Models.NotificationType.NewMessage,
                "New Message",
                !string.IsNullOrEmpty(message.Content)
                    ? (message.Content.Length > 100 ? message.Content[..100] + "..." : message.Content)
                    : "You received a new message.",
                $"/messages/{message.SenderId}",
                cancellationToken);

            return dto;
        }
    }
}
