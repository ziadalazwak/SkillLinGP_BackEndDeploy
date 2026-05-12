using MediatR;

namespace SkillLink.Application.UseCases.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }

        // Profile image fields — populated by the API layer from IFormFile
        public Stream? ProfileImageStream { get; set; }
        public string? ProfileImageFileName { get; set; }
    }
}
