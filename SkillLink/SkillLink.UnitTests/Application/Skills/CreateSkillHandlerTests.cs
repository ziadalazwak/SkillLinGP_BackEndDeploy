using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Skills.Commands.CreateSkill;
using SkillLink.Infrastructure;

namespace SkillLink.UnitTests.Application.Skills
{
    public class CreateSkillHandlerTests
    {
        private readonly SkillLinkDbContext _context;
        private readonly Mock<IFileStorageService> _fileStorageMock;
        private readonly CreateSkillHandler _handler;

        public CreateSkillHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SkillLinkDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SkillLinkDbContext(options);
            _fileStorageMock = new Mock<IFileStorageService>();
            
            _handler = new CreateSkillHandler(_context, _fileStorageMock.Object);
        }

        [Fact]
        public async Task Handle_WithNoImage_CreatesSkillAndReturnsId()
        {
            // Arrange
            var command = new CreateSkillCommand
            {
                Title = "Test Skill",
                Description = "Test Description",
                CategoryId = 1,
                ImageBytes = null,
                ImageExtension = null
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var skill = await _context.Skills.FindAsync(result);
            skill.Should().NotBeNull();
            skill!.Title.Should().Be("Test Skill");
            skill.Description.Should().Be("Test Description");
            skill.CategoryId.Should().Be(1);
            skill.ImageUrl.Should().BeNull();
            
            _fileStorageMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithImage_UploadsImageAndCreatesSkillWithUrl()
        {
            // Arrange
            var expectedUrl = "https://example.com/images/skill.jpg";
            _fileStorageMock
                .Setup(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), "skills", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUrl);

            var command = new CreateSkillCommand
            {
                Title = "Test Skill With Image",
                Description = "Test Description",
                CategoryId = 2,
                ImageBytes = new byte[] { 0x1, 0x2, 0x3 },
                ImageExtension = ".jpg"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var skill = await _context.Skills.FindAsync(result);
            skill.Should().NotBeNull();
            skill!.Title.Should().Be("Test Skill With Image");
            skill.ImageUrl.Should().Be(expectedUrl);

            _fileStorageMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), "skill.jpg", "skills", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
