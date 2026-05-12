using FluentValidation;

namespace SkillLink.Application.UseCases.Sessions.Commands.CreateSession
{
    public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionCommandValidator()
        {
            RuleFor(x => x.RequesterId)
                .GreaterThan(0).WithMessage("Requester ID must be a valid positive integer.");
                
            RuleFor(x => x.SkillId)
                .GreaterThan(0).WithMessage("Skill ID must be a valid positive integer.");
                
        
                
            RuleFor(x => x.ProviderId)
                .NotEqual(x => x.RequesterId).WithMessage("Requester and Provider cannot be the same user.");
                
            RuleFor(x => x.ScheduledAt)
                .GreaterThan(System.DateTime.UtcNow).WithMessage("Scheduled date must be in the future.");
                
            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("Note cannot exceed 500 characters.");
        }
    }
}
