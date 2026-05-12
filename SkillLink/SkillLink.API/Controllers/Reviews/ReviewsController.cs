using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.Application.UseCases.Reviews.Commands.CreateReview;
using SkillLink.Application.UseCases.Reviews.Queries.GetReviewById;

namespace SkillLink.API.Controllers.Reviews
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
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
        public async Task<IActionResult> CreateReview([FromBody] SkillLink.API.ApiDto.CreateReviewRequestDto request)
        {
            try
            {
                var command = new CreateReviewCommand
                {
                    ReviewerId   = GetCurrentUserId(),
                    RevieweeId   = request.RevieweeId,
                    SessionId    = request.SessionId,
                    SkillTradeId = request.SkillTradeId,
                    Rating       = request.Rating,
                    Comment      = request.Comment
                };

                var reviewId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetReview), new { id = reviewId }, new { id = reviewId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            var userId = GetCurrentUserId();

            var query = new GetReviewByIdQuery(userId);
            var result = await _mediator.Send(query);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
