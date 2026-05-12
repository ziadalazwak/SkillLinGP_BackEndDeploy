using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.API.ApiDto;
using SkillLink.Application.UseCases.Trades.Commands.AcceptTrade;
using SkillLink.Application.UseCases.Trades.Commands.CancelTrade;
using SkillLink.Application.UseCases.Trades.Commands.CompleteTrade;
using SkillLink.Application.UseCases.Trades.Commands.CreateTrade;
using SkillLink.Application.UseCases.Trades.Commands.RejectTrade;
using SkillLink.Application.UseCases.Trades.Queries.GetTradeById;
using SkillLink.Application.UseCases.Trades.Queries.GetTrades;

namespace SkillLink.API.Controllers.Trades
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TradesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TradesController(IMediator mediator)
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
        public async Task<IActionResult> CreateTrade([FromBody] CreateTradeDto request)
        {
            var command = new CreateTradeCommand
            {
                OffererId        = GetCurrentUserId(),
                ReceiverId       = request.ReceiverId,
                OfferedSkillId   = request.OfferedSkillId,
                RequestedSkillId = request.RequestedSkillId,
                Message          = request.Message
            };

            var tradeId = await _mediator.Send(command);
            return StatusCode(201, new { Id = tradeId });
        }

        [HttpGet]
        public async Task<IActionResult> GetTrades([FromQuery] GetTradesQuery request)
        {
            request.UserId = GetCurrentUserId();
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrade(int id)
        {
            var query = new GetTradeByIdQuery(id, GetCurrentUserId());
            var result = await _mediator.Send(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptTrade(int id)
        {
            var command = new AcceptTradeCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectTrade(int id)
        {
            var command = new RejectTradeCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTrade(int id)
        {
            var command = new CompleteTradeCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelTrade(int id)
        {
            var command = new CancelTradeCommand(id, GetCurrentUserId());
            var success = await _mediator.Send(command);

            if (!success) return NotFound();
            return Ok();
        }
    }
}
