using FluentValidation;

namespace SkillLink.Application.UseCases.Trades.Commands.CreateTrade
{
    public class CreateTradeCommandValidator : AbstractValidator<CreateTradeCommand>
    {
        public CreateTradeCommandValidator()
        {
            RuleFor(x => x.OffererId)
                .GreaterThan(0).WithMessage("Offerer ID must be a positive integer.");
                
            RuleFor(x => x.ReceiverId)
                .GreaterThan(0).WithMessage("Receiver ID must be a positive integer.");
                
            RuleFor(x => x.ReceiverId)
                .NotEqual(x => x.OffererId).WithMessage("Offerer and Receiver cannot be the same user.");
                
            RuleFor(x => x.OfferedSkillId)
                .GreaterThan(0).WithMessage("Offered Skill ID must be a positive integer.");
                
            RuleFor(x => x.RequestedSkillId)
                .GreaterThan(0).WithMessage("Requested Skill ID must be a positive integer.");
                
            RuleFor(x => x.Message)
                .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");
        }
    }
}
