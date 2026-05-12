using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.API.ApiDto;
using SkillLink.Application.UseCases.Sessions.Commands.AcceptSession;
using SkillLink.Application.UseCases.Sessions.Commands.CancelSession;
using SkillLink.Application.UseCases.Sessions.Commands.CompleteSession;
using SkillLink.Application.UseCases.Sessions.Commands.CreateSession;
using SkillLink.Application.UseCases.Sessions.Commands.RejectSession;
using SkillLink.Application.UseCases.Sessions.Queries.GetSessionById;
using SkillLink.Application.UseCases.Sessions.Queries.GetSessions;

namespace SkillLink.API.Controllers.Sessions
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionsController(IMediator mediator)
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

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionDto request)
        {
            var command = new CreateSessionCommand
            {
                RequesterId              = GetCurrentUserId(),
                SkillId                  = request.SkillId,
                ProviderId               = request.ProviderId,
                ScheduledAt              = request.ScheduledAt,
                Note                     = request.Note,
                RequestedDurationMinutes = request.RequestedDurationMinutes
            };

            var sessionId = await _mediator.Send(command);
            return StatusCode(201, new { Id = sessionId });
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions([FromQuery] GetSessionsQuery request)
        {
            request.UserId = GetCurrentUserId();
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var query = new GetSessionByIdQuery(id, GetCurrentUserId());
            var result = await _mediator.Send(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptSession(int id)
        {
            var command = new AcceptSessionCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectSession(int id, [FromBody] SessionReasonDto request)
        {
            var command = new RejectSessionCommand(id, GetCurrentUserId(), request.Reason);
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteSession(int id)
        {
            var command = new CompleteSessionCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelSession(int id, [FromBody] SessionReasonDto request)
        {
            var command = new CancelSessionCommand(id, GetCurrentUserId(), request.Reason);
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }
    }
}
