using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.Application.UseCases.Credits.Queries.GetCreditBalance;
using SkillLink.Application.UseCases.Credits.Queries.GetCreditTransactions;
using SkillLink.Domain.Models;

namespace SkillLink.API.Controllers.Credits
{
    [ApiController]
    [Route("api/credits")]
    [Authorize]
    public class CreditsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CreditsController(IMediator mediator)
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

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var query = new GetCreditBalanceQuery(GetCurrentUserId());
            var result = await _mediator.Send(query);
            return Ok(new { balance = result });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] TransactionType? type, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1)
        {
            var query = new GetCreditTransactionsQuery
            {
                UserId   = GetCurrentUserId(),
                Type     = type,
                From     = from,
                To       = to,
                Page     = page,
                PageSize = 20
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
