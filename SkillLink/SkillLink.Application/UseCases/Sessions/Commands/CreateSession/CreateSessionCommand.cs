using MediatR;
using System;

namespace SkillLink.Application.UseCases.Sessions.Commands.CreateSession
{
    public record CreateSessionCommand : IRequest<int>
    {
        public int RequesterId { get; init; }
        public int SkillId { get; init; }
        public int ProviderId { get; init; }
        public DateTime ScheduledAt { get; init; }
        public string? Note { get; init; }

        /// <summary>
        /// Requested session duration chosen by the requester (30–180 minutes, i.e. 0.5–3 hours).
        /// </summary>
        public int RequestedDurationMinutes { get; init; }
    }
}
