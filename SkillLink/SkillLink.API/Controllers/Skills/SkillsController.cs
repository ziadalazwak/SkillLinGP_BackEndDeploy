using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.API.ApiDto;
using SkillLink.Application.UseCases.Skills.Commands.CreateSkill;
using SkillLink.Application.UseCases.Skills.Commands.DeleteSkill;
using SkillLink.Application.UseCases.Skills.Commands.UpdateSkill;
using SkillLink.Application.UseCases.Skills.Commands.GenerateSkillEmbeddings;
using SkillLink.Application.UseCases.Skills.Queries.GetSKills;
using SkillLink.Application.UseCases.Skills.Queries.GetActiveSkills;
using SkillLink.Application.UseCases.Skills.Queries.GetSkillById;

namespace SkillLink.API.Controllers.Skills
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SkillsController(IMediator mediator)
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
        [AllowAnonymous]
        public async Task<IActionResult> GetSkills([FromQuery] GetSkillsQuery query)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                query.CurrentUserId = userId;
            }

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveSkills()
        {
            var result = await _mediator.Send(new GetActiveSkillsQuery());
            return Ok(result);
        }
        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSkills()
        {
            var result = await _mediator.Send(new GetActiveSkillsQuery());
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var result = await _mediator.Send(new GetSkillByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("generate-embeddings")]
     
        public async Task<IActionResult> GenerateEmbeddings()
        {
            var count = await _mediator.Send(new GenerateSkillEmbeddingsCommand());
            return Ok(new { GeneratedCount = count });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSkill([FromForm] CreateSkillDto request)
        {
            byte[]? imageBytes = null;
            string? imageExtension = null;

            if (request.Image is not null)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms);
                imageBytes     = ms.ToArray();
                imageExtension = Path.GetExtension(request.Image.FileName).ToLower();
            }

            var command = new CreateSkillCommand
            {
                Title          = request.Title,
                Description    = request.Description,
                CategoryId     = request.CategoryId,
                ImageBytes     = imageBytes,
                ImageExtension = imageExtension,
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSkill(int id, [FromForm] UpdateSkillDto request)
        {
            byte[]? imageBytes = null;
            string? imageExtension = null;

            if (request.Image is not null)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms);
                imageBytes     = ms.ToArray();
                imageExtension = Path.GetExtension(request.Image.FileName).ToLower();
            }

            var command = new UpdateSkillCommand
            {
                Id             = id,
                ProviderId     = GetCurrentUserId(),
                Title          = request.Title,
                Description    = request.Description,
                IsActive       = request.IsActive,
                ImageBytes     = imageBytes,
                ImageExtension = imageExtension,
            };

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var command = new DeleteSkillCommand(id, GetCurrentUserId());

            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
