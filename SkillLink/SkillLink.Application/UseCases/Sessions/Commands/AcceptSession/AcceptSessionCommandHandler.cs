using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Sessions.Commands.AcceptSession
{
    public class AcceptSessionCommandHandler : IRequestHandler<AcceptSessionCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly INotificationService _notificationService;

        public AcceptSessionCommandHandler(ISkillLinkDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(AcceptSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null) return false;

            if (session.ProviderId != request.ProviderId)
            {
                throw new UnauthorizedAccessException("Only the provider can accept the session.");
            }

            if (session.Status != SessionStatus.Pending)
            {
                throw new InvalidOperationException("Session is not in Pending state.");
            }

            session.Status = SessionStatus.Accepted;
            session.MeetingLink = $"https://meet.jit.si/SkillLink-Session-{session.Id}-{Guid.NewGuid():N}";

            _context.Sessions.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendNotificationAsync(
                session.RequesterId,
                NotificationType.SessionAccepted,
                "Session Accepted",
                "Your session request has been accepted! A meeting link has been generated.",
                $"/sessions/{session.Id}",
                cancellationToken);

            return true;
        }
    }
}
