using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.Application.UseCases.Skills.Queries.GetUserSkills;

namespace SkillLink.API.Controllers.Users
{
    [ApiController]
    [Route("api/users/me/skills")]
    [Authorize]
    public class UserSkillsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserSkillsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID");
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySkills()
        {
            int providerId = GetCurrentUserId();
            var query = new GetUserSkillsQuery(providerId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("GetUsersSkillsProfile/{id}")]
        public async Task<IActionResult> GetUsersSkillsProfile(int id )
        {
            var query = new GetUserSkillsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Get a single user-skill entry by skill ID for the authenticated user.</summary>
        [HttpGet("{skillId}")]
        public async Task<IActionResult> GetMySkillById(int skillId)
        {
            var userId = GetCurrentUserId();
            var query = new GetUserSkillByIdQuery(userId, skillId);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Get a single user-skill entry by skill ID for any user.</summary>
        [HttpGet("{userId}/skills/{skillId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserSkillById(int userId, int skillId)
        {
            var query = new GetUserSkillByIdQuery(userId, skillId);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddUserSkill([FromBody] SkillLink.API.ApiDto.AddUserSkillDto request)
        {
            request.UserId = GetCurrentUserId();
            var command = new SkillLink.Application.UseCases.Skills.Commands.AddUserSkill.AddUserSkillCommand(
                request.UserId,
                request.SkillId,
                request.Level,
                request.ExchangeMode,
                request.DurationMinutes,
                request.DeliveryMode,
                request.Notes
            );

            var success = await _mediator.Send(command);
            if (!success) return BadRequest("Could not add skill.");

            return Ok();
        }

        [HttpDelete("{skillId}")]
        public async Task<IActionResult> RemoveUserSkill(int skillId)
        {
            var userId = GetCurrentUserId();
            var command = new SkillLink.Application.UseCases.Skills.Commands.RemoveUserSkill.RemoveUserSkillCommand(userId, skillId);
            var success = await _mediator.Send(command);

            if (!success) return NotFound();

            return Ok();
        }
    }
}
