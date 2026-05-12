using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System.Security.Authentication;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessionById
{
    public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, SessionDetailDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetSessionByIdQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<SessionDetailDto?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions
                .Include(s => s.Skill)
                .Include(s => s.Provider)
                .Include(s => s.Requester)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (session == null) return null;

            // Security: Only participants can view full details
            if (session.RequesterId != request.UserId && session.ProviderId != request.UserId)
            {
                throw new AuthenticationException("You are not a participant in this session.");
            }

            return new SessionDetailDto(
                session.Id,
                session.SkillId,
                session.Skill?.Title ?? "Unknown Skill",
                session.Skill?.Description,
                session.ProviderId,
                session.Provider?.FullName ?? "Unknown Provider",
                session.Provider?.ProfilePictureUrl,
                session.RequesterId,
                session.Requester?.FullName ?? "Unknown Requester",
                session.Requester?.ProfilePictureUrl,
                session.Status,
                session.ScheduledAt,
                session.DurationMinutes,
                session.CreditCost,
                session.MeetingLink,
                session.RequesterNote,
                session.ProviderNote,
                session.CreatedAt,

                session.CompletedAt
            );
        }
    }
}
