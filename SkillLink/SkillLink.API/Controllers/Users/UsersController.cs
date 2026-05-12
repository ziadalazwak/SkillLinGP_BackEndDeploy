using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SkillLink.Application.UseCases.Users.Queries.GetUserProfile;
using SkillLink.Application.UseCases.Users.Queries.GetPublicUserProfile;
using SkillLink.Application.UseCases.Users.Queries.GetUserReviews;
using SkillLink.Application.UseCases.Users.Queries.GetRecommendedSkills;
using SkillLink.Application.UseCases.Users.Commands.UpdateUserProfile;
using SkillLink.Application.UseCases.Users.Commands.UploadAvatar;
using SkillLink.Application.UseCases.Admin.Queries.GetAllUsers;
using SkillLink.Application.UseCases.Admin.Commands.BanUser;
using SkillLink.Application.UseCases.Admin.Commands.UnbanUser;
using SkillLink.Application.UseCases.Admin.Commands.DeleteUser;

namespace SkillLink.API.Controllers.Users
{
    /// <summary>API-layer form DTO for profile editing. All fields are optional —
    /// only the ones provided will be updated.</summary>
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };

        public UsersController(IMediator mediator)
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

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _mediator.Send(new GetUserProfileQuery(GetCurrentUserId()));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("me/recommendations")]
        public async Task<IActionResult> GetMyRecommendations([FromQuery] int topN = 5)
        {
            var result = await _mediator.Send(new GetRecommendedSkillsQuery(GetCurrentUserId(), topN));
            return Ok(result);
        }

        /// <summary>
        /// Update the current user's profile. All fields are optional.
        /// Send as multipart/form-data; include ProfileImage to change the avatar in the same request.
        /// </summary>
        [HttpPut("me")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateProfileRequest request)
        {
            if (request.ProfileImage != null)
            {
                if (request.ProfileImage.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Profile image must not exceed 5 MB." });

                if (!AllowedImageTypes.Contains(request.ProfileImage.ContentType.ToLower()))
                    return BadRequest(new { message = "Only JPEG, PNG, WebP or GIF images are allowed." });
            }

            Stream? imageStream = request.ProfileImage?.OpenReadStream();

            var command = new UpdateUserProfileCommand
            {
                UserId = GetCurrentUserId(),
                FullName = request.FullName,
                Bio = request.Bio,
                PhoneNumber = request.PhoneNumber,
                ProfileImageStream = imageStream,
                ProfileImageFileName = request.ProfileImage?.FileName
            };

            var success = await _mediator.Send(command);
            imageStream?.Dispose();

            if (!success) return NotFound(new { message = "User not found." });

            var updatedProfile = await _mediator.Send(new GetUserProfileQuery(command.UserId));
            return Ok(updatedProfile);
        }

        /// <summary>
        /// Upload / replace the current user's profile picture (dedicated endpoint).
        /// Kept for backward compatibility — PUT /me also supports image upload.
        /// </summary>
        [HttpPost("me/avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "File is empty." });
            if (file.Length > 5 * 1024 * 1024) return BadRequest(new { message = "File size exceeds 5 MB limit." });

            var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowed.Contains(file.ContentType.ToLower()))
                return BadRequest(new { message = "Only JPEG, PNG, WebP or GIF images are allowed." });

            using var stream = file.OpenReadStream();
            var avatarCommand = new UploadAvatarCommand
            {
                UserId = GetCurrentUserId(),
                FileStream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            var url = await _mediator.Send(avatarCommand);
            if (url == null) return BadRequest(new { message = "Could not upload avatar." });

            return Ok(new { profilePictureUrl = url });
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPublicProfile(int id)
        {
            var result = await _mediator.Send(new GetPublicUserProfileQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/reviews")]
        public async Task<IActionResult> GetUserReviews(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetUserReviewsQuery(id, page, pageSize));
            return Ok(result);
        }

        // ── Admin endpoints ──────────────────────────────────────────────────

        /// <summary>Get all users (admin only).</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Deactivate (ban) a user (admin only).</summary>
        [HttpPost("{id:int}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(int id)
        {
            var success = await _mediator.Send(new BanUserCommand(id));
            if (!success) return NotFound(new { message = "User not found." });
            return Ok(new { message = "User has been deactivated." });
        }

        /// <summary>Reactivate (unban) a user (admin only).</summary>
        [HttpPost("{id:int}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnbanUser(int id)
        {
            var success = await _mediator.Send(new UnbanUserCommand(id));
            if (!success) return NotFound(new { message = "User not found." });
            return Ok(new { message = "User has been reactivated." });
        }

        /// <summary>Soft-delete a user (admin only).</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _mediator.Send(new DeleteUserCommand(id));
            if (!success) return NotFound(new { message = "User not found." });
            return NoContent();
        }
    }
}
