using FluentValidation;

namespace SkillLink.Application.UseCases.Skills.Commands.CreateSkill
{
    public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
    {
        public CreateSkillCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be a valid positive integer.");
                
            RuleFor(x => x.ImageExtension)
                .Must(ext => string.IsNullOrEmpty(ext) || ValidImageExtensions.Contains(ext.ToLower()))
                .WithMessage("If an image is provided, extension must be .jpg, .jpeg, or .png");
        }
        
        private static readonly string[] ValidImageExtensions = { ".jpg", ".jpeg", ".png" };
    }
}
