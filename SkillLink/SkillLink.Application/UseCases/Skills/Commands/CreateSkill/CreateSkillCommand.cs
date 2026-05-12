using MediatR;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Skills.Commands.CreateSkill
{
    public record CreateSkillCommand : IRequest<int>
    {
        // ── Skill data ─────────────────────────────────────────────────────
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int CategoryId { get; init; }
      

        // ── File data (raw — no IFormFile, no URL) ─────────────────────────
        public byte[]? ImageBytes { get; init; }
        public string? ImageExtension { get; init; }   // e.g. ".jpg"
    }
}
