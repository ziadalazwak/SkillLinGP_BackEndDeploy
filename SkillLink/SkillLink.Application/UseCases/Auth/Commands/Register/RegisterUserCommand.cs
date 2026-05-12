using MediatR;

namespace SkillLink.Application.UseCases.Auth.Commands.Register
{
    public class RegisterUserCommand : IRequest<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Profile image fields — populated by the API layer from IFormFile
        public Stream? ProfileImageStream { get; set; }
        public string? ProfileImageFileName { get; set; }

        /// <summary>When true the created domain user will have IsAdmin = true.</summary>
        public bool IsAdmin { get; set; } = false;
    }
}
