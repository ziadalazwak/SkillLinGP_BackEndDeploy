using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.Application.UseCases.Notifications.Commands.MarkAllNotificationsRead;
using SkillLink.Application.UseCases.Notifications.Commands.MarkNotificationRead;
using SkillLink.Application.UseCases.Notifications.Queries.GetUnreadNotificationCount;
using SkillLink.Application.UseCases.Notifications.Queries.GetUserNotifications;

namespace SkillLink.API.Controllers.Notifications
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
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
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new GetUserNotificationsQuery(userId, page, pageSize));
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _mediator.Send(new GetUnreadNotificationCountQuery(userId));
            return Ok(new { UnreadCount = count });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _mediator.Send(new MarkNotificationReadCommand(id, userId));
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var count = await _mediator.Send(new MarkAllNotificationsReadCommand(userId));
            return Ok(new { MarkedCount = count });
        }
    }
}
