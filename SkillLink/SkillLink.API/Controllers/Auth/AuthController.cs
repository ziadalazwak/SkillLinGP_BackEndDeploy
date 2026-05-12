using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillLink.Application.UseCases.Auth.Commands.Account;
using SkillLink.Application.UseCases.Auth.Commands.Login;
using SkillLink.Application.UseCases.Auth.Commands.Refresh;
using SkillLink.Application.UseCases.Auth.Commands.Register;

namespace SkillLink.API.Controllers.Auth
{
    /// <summary>API-layer DTO for registration. Accepts multipart/form-data so the
    /// profile image can be uploaded together with the text fields.</summary>
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            try
            {
                if (request.ProfileImage != null)
                {
                    if (request.ProfileImage.Length > 5 * 1024 * 1024)
                        return BadRequest(new { errors = new[] { "Profile image must not exceed 5 MB." } });

                    var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
                    if (!allowed.Contains(request.ProfileImage.ContentType.ToLower()))
                        return BadRequest(new { errors = new[] { "Only JPEG, PNG, WebP or GIF images are allowed." } });
                }

                // Build the application command, passing streams (not IFormFile) into the Application layer
                Stream? imageStream = request.ProfileImage != null ? request.ProfileImage.OpenReadStream() : null;
                var command = new RegisterUserCommand
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    ProfileImageStream = imageStream,
                    ProfileImageFileName = request.ProfileImage?.FileName
                };

                var userId = await _mediator.Send(command);

                imageStream?.Dispose();

                return CreatedAtAction(nameof(Register), new { id = userId }, new { userId, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }
        }

        [HttpPost("register-admin")]
    
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterAdmin([FromForm] RegisterRequest request)
        {
            try
            {
                if (request.ProfileImage != null)
                {
                    if (request.ProfileImage.Length > 5 * 1024 * 1024)
                        return BadRequest(new { errors = new[] { "Profile image must not exceed 5 MB." } });

                    var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
                    if (!allowed.Contains(request.ProfileImage.ContentType.ToLower()))
                        return BadRequest(new { errors = new[] { "Only JPEG, PNG, WebP or GIF images are allowed." } });
                }

                Stream? imageStream = request.ProfileImage != null ? request.ProfileImage.OpenReadStream() : null;
                var command = new RegisterUserCommand
                {
                    FullName             = request.FullName,
                    Email                = request.Email,
                    Password             = request.Password,
                    PhoneNumber          = request.PhoneNumber,
                    ProfileImageStream   = imageStream,
                    ProfileImageFileName = request.ProfileImage?.FileName,
                    IsAdmin              = true
                };

                var userId = await _mediator.Send(command);

                imageStream?.Dispose();

                return CreatedAtAction(nameof(RegisterAdmin), new { id = userId },
                    new { userId, message = "Admin registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex) when (ex.Message == "EMAIL_NOT_VERIFIED")
            {
                return Unauthorized(new
                {
                    message = "Email not verified. Please check your inbox for the OTP and verify your account.",
                    requiresOtpVerification = true,
                    email = command.Email
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials or deactivated account." });
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserTokenCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { message = "Email verified successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await _mediator.Send(command);
            // Always return HTTP 200 to prevent user enumeration
            return Ok(new { message = "If the email is registered, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutUserCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtpEmail([FromBody] SendOtpCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { message = "Verification email sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
            [HttpPost("VerifyOtp")]
            public async Task<IActionResult> VerifyOtpEmail([FromBody] VerifyOtpCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "OTP verified successfully." });
        }
    }
}
