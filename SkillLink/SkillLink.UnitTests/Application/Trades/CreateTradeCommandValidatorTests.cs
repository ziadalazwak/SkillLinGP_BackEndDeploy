using FluentValidation.TestHelper;
using SkillLink.Application.UseCases.Trades.Commands.CreateTrade;

namespace SkillLink.UnitTests.Application.Trades
{
    public class CreateTradeCommandValidatorTests
    {
        private readonly CreateTradeCommandValidator _validator;

        public CreateTradeCommandValidatorTests()
        {
            _validator = new CreateTradeCommandValidator();
        }

        [Fact]
        public void Validate_SameOffererAndReceiver_ShouldHaveValidationError()
        {
            var command = new CreateTradeCommand
            {
                OffererId = 1,
                ReceiverId = 1,
                OfferedSkillId = 1,
                RequestedSkillId = 2
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ReceiverId)
                .WithErrorMessage("Offerer and Receiver cannot be the same user.");
        }

        [Fact]
        public void Validate_NegativeIds_ShouldHaveValidationError()
        {
            var command = new CreateTradeCommand
            {
                OffererId = 0,
                ReceiverId = -1,
                OfferedSkillId = 0,
                RequestedSkillId = -5
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.OffererId);
            result.ShouldHaveValidationErrorFor(x => x.ReceiverId);
            result.ShouldHaveValidationErrorFor(x => x.OfferedSkillId);
            result.ShouldHaveValidationErrorFor(x => x.RequestedSkillId);
        }

        [Fact]
        public void Validate_MessageTooLong_ShouldHaveValidationError()
        {
            var command = new CreateTradeCommand
            {
                OffererId = 1,
                ReceiverId = 2,
                OfferedSkillId = 1,
                RequestedSkillId = 2,
                Message = new string('a', 501)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Message)
                .WithErrorMessage("Message cannot exceed 500 characters.");
        }

        [Fact]
        public void Validate_ValidCommand_ShouldNotHaveAnyErrors()
        {
            var command = new CreateTradeCommand
            {
                OffererId = 1,
                ReceiverId = 2,
                OfferedSkillId = 1,
                RequestedSkillId = 2,
                Message = "Help me learn Java"
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
