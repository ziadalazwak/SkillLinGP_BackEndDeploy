using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.API.ApiDto;
using SkillLink.Application.UseCases.Messages.Commands.DeleteMessage;
using SkillLink.Application.UseCases.Messages.Commands.MarkMessagesRead;
using SkillLink.Application.UseCases.Messages.Commands.SendMessage;
using SkillLink.Application.UseCases.Messages.Queries.GetConversations;
using SkillLink.Application.UseCases.Messages.Queries.GetMessageThread;
using SkillLink.Application.UseCases.Messages.Queries.GetUnreadMessageCount;

namespace SkillLink.API.Controllers.Messages
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetUserId()
        {
            var claimsId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claimsId, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity.");
            return userId;
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            var count = await _mediator.Send(new GetUnreadMessageCountQuery(userId));
            return Ok(new { UnreadCount = count });
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new GetConversationsQuery(userId));
            return Ok(result);
        }

        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetMessageThread(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            var currentUserId = GetUserId();
            var result = await _mediator.Send(new GetMessageThreadQuery(currentUserId, userId, page, pageSize));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageRequestDto request)
        {
            var currentUserId = GetUserId();

            var command = new SendMessageCommand
            {
                SenderId   = currentUserId,
                ReceiverId = request.ReceiverId,
                SessionId  = request.SessionId,
                Content    = request.Content
            };

            if (request.AttachmentFile != null)
            {
                command.AttachmentStream   = request.AttachmentFile.OpenReadStream();
                command.AttachmentFileName = request.AttachmentFile.FileName;
            }

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMessageThread), new { userId = request.ReceiverId }, result);
        }

        [HttpPost("conversations/{userId}/read")]
        public async Task<IActionResult> MarkMessagesRead(int userId)
        {
            var currentUserId = GetUserId();
            await _mediator.Send(new MarkMessagesReadCommand(currentUserId, userId));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var currentUserId = GetUserId();
            await _mediator.Send(new DeleteMessageCommand(id, currentUserId));
            return NoContent();
        }
    }
}
