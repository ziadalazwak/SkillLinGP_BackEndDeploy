using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Sessions.Commands.CreateSession;
using SkillLink.Domain.Models;
using SkillLink.Infrastructure;

namespace SkillLink.UnitTests.Application.Sessions
{
    public class CreateSessionCommandHandlerTests
    {
        private readonly SkillLinkDbContext _context;
        private readonly CreateSessionCommandHandler _handler;

        public CreateSessionCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SkillLinkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SkillLinkDbContext(options);
            var notificationService = new Mock<INotificationService>().Object;
            _handler = new CreateSessionCommandHandler(_context, notificationService);
        }

        [Fact]
        public async Task Handle_SameRequesterAndProvider_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateSessionCommand { RequesterId = 1, ProviderId = 1, RequestedDurationMinutes = 60 };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot request a session with yourself.");
        }

        [Fact]
        public async Task Handle_ProviderDoesNotOfferSkill_ThrowsInvalidOperationException()
        {
            // Arrange
            var skill = new Skill { Id = 1, Title = "React", IsActive = true };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            var command = new CreateSessionCommand { RequesterId = 2, ProviderId = 1, SkillId = 1, RequestedDurationMinutes = 60 };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The specified provider does not offer this skill.");
        }

        [Fact]
        public async Task Handle_InsufficientCredits_ThrowsInvalidOperationException()
        {
            // Arrange
            var skill = new Skill { Id = 1, Title = "React", IsActive = true };
            _context.Skills.Add(skill);
            // Fixed rate: 180 min = 3 whole-hour credits. User only has 1.
            _context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, IsOffering = true, ExchangeMode = ExchangeMode.CreditBased, DurationMinutes = 180 });
            var user = new User { Id = 2 };
            user.AdjustCreditBalance(1); // Only 1, needs 3
            _context.DomainUsers.Add(user);
            await _context.SaveChangesAsync();

            var command = new CreateSessionCommand { RequesterId = 2, ProviderId = 1, SkillId = 1, RequestedDurationMinutes = 180 };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Insufficient credit balance to hold.");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesSessionAndReturnsId()
        {
            // Arrange
            var skill = new Skill { Id = 2, Title = "Node.js", IsActive = true };
            _context.Skills.Add(skill);
            // Fixed rate: credits are whole hours. 60 min rounds to 1 credit.
            _context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 2, IsOffering = true, ExchangeMode = ExchangeMode.CreditBased, DurationMinutes = 60 });
            var user = new User { Id = 3 };
            user.AdjustCreditBalance(10);
            _context.DomainUsers.Add(user);
            await _context.SaveChangesAsync();

            var command = new CreateSessionCommand
            {
                RequesterId              = 3,
                ProviderId               = 1,
                SkillId                  = 2,
                ScheduledAt              = DateTime.UtcNow.AddDays(1),
                RequestedDurationMinutes = 60
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var session = await _context.Sessions.FindAsync(result);
            session.Should().NotBeNull();
            session!.RequesterId.Should().Be(3);
            session.ProviderId.Should().Be(1);
            session.SkillId.Should().Be(2);
            session.CreditCost.Should().Be(1);    // 60 min rounds to 1 whole-hour credit
            session.DurationMinutes.Should().Be(60);
            session.Status.Should().Be(SessionStatus.Pending);
        }
    }
}
