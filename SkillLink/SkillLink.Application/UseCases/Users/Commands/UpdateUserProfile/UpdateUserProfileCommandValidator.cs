using FluentValidation;

namespace SkillLink.Application.UseCases.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
                .When(x => x.FullName != null);

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.")
                .When(x => x.Bio != null);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("Phone number format is invalid.")
                .When(x => x.PhoneNumber != null);

            RuleFor(x => x.ProfileImageFileName)
                .Must(name => name == null || AllowedExtensions.Contains(
                    System.IO.Path.GetExtension(name).ToLower()))
                .WithMessage("Profile image must be a JPEG, PNG, WebP or GIF file.")
                .When(x => x.ProfileImageFileName != null);
        }
    }
}
