using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Messages.Commands.DeleteMessage
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
    {
        private readonly ISkillLinkDbContext _context;

        public DeleteMessageCommandHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == request.MessageId && !m.IsDeleted, cancellationToken);

            if (message == null) throw new InvalidOperationException("Message not found.");

            if (message.SenderId != request.CurrentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own messages.");
            }

            var timeSinceSent = DateTime.UtcNow - message.SentAt;
            if (timeSinceSent.TotalMinutes > 5)
            {
                throw new InvalidOperationException("You can only delete messages within 5 minutes of sending them.");
            }

            message.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
