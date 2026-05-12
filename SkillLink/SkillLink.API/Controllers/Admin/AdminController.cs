using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillLink.Application.UseCases.Admin.Commands.ToggleSkillActive;
using SkillLink.Application.UseCases.Admin.Queries.GetAllSkills;
using SkillLink.Application.UseCases.Admin.Queries.GetAllSessions;

namespace SkillLink.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ── Skills ────────────────────────────────────────────────────────────

        /// <summary>Get all skills regardless of owner or active state (admin only).</summary>
        [HttpGet("skills")]
        public async Task<IActionResult> GetAllSkills([FromQuery] GetAllSkillsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Toggle a skill's IsActive flag (admin only).</summary>
        [HttpPatch("skills/{id:int}/toggle-active")]
        public async Task<IActionResult> ToggleSkillActive(int id)
        {
            var success = await _mediator.Send(new ToggleSkillActiveCommand(id));
            if (!success) return NotFound(new { message = "Skill not found." });
            return Ok(new { message = "Skill active status toggled." });
        }

        // ── Sessions ──────────────────────────────────────────────────────────

        /// <summary>Get all sessions across all users (admin only).</summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetAllSessions([FromQuery] GetAllSessionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
