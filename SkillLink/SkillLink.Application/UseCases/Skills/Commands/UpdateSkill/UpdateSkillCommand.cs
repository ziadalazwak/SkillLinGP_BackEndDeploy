using MediatR;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Skills.Commands.UpdateSkill
{
    public record UpdateSkillCommand : IRequest<bool>
    {
        public int Id { get; init; }
        
        // This should be mapped from the claims (the user who made the request)
        public int ProviderId { get; init; }

        public string? Title { get; init; }
        public string? Description { get; init; }
        public bool? IsActive { get; init; }

        // ── Image (raw — no IFormFile) ─────────────────────────────────────
        public byte[]? ImageBytes { get; init; }
        public string? ImageExtension { get; init; }   // e.g. ".jpg"
    }
}
