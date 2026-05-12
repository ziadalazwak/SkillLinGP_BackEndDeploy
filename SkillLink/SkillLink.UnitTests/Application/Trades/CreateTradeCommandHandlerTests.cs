using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Trades.Commands.CreateTrade;
using SkillLink.Domain.Models;
using SkillLink.Infrastructure;

namespace SkillLink.UnitTests.Application.Trades
{
    public class CreateTradeCommandHandlerTests
    {
        private readonly SkillLinkDbContext _context;
        private readonly CreateTradeCommandHandler _handler;

        public CreateTradeCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SkillLinkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SkillLinkDbContext(options);
            var notificationService = new Mock<INotificationService>().Object;
            _handler = new CreateTradeCommandHandler(_context, notificationService);
        }

        [Fact]
        public async Task Handle_SameOffererAndReceiver_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateTradeCommand { OffererId = 1, ReceiverId = 1 };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot trade with yourself.");
        }

        [Fact]
        public async Task Handle_SkillsDoNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var command = new CreateTradeCommand
            {
                OffererId = 1,
                ReceiverId = 2,
                OfferedSkillId = 99,
                RequestedSkillId = 100
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("One or both skills were not found or are inactive.");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesTradeAndReturnsId()
        {
            // Arrange
            var offeredSkill = new Skill { Id = 1, Title = "C#", IsActive = true };
            var requestedSkill = new Skill { Id = 2, Title = "Java", IsActive = true };
            
            _context.Skills.AddRange(offeredSkill, requestedSkill);
            
            _context.UserSkills.Add(new UserSkill { UserId = 1, SkillId = 1, IsOffering = true });
            _context.UserSkills.Add(new UserSkill { UserId = 2, SkillId = 2, IsOffering = true });
            
            await _context.SaveChangesAsync();

            var command = new CreateTradeCommand
            {
                OffererId = 1,
                ReceiverId = 2,
                OfferedSkillId = 1,
                RequestedSkillId = 2
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var trade = await _context.SkillTrades.FindAsync(result);
            trade.Should().NotBeNull();
            trade!.OffererId.Should().Be(1);
            trade.ReceiverId.Should().Be(2);
            trade.OfferedSkillId.Should().Be(1);
            trade.RequestedSkillId.Should().Be(2);
            trade.Status.Should().Be(TradeStatus.Pending);
        }
    }
}
