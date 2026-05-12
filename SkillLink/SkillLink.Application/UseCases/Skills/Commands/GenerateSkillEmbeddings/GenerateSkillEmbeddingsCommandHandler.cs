using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System.Text.Json;

namespace SkillLink.Application.UseCases.Skills.Commands.GenerateSkillEmbeddings
{
    public class GenerateSkillEmbeddingsCommandHandler : IRequestHandler<GenerateSkillEmbeddingsCommand, int>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IEmbeddingService _embeddingService;

        public GenerateSkillEmbeddingsCommandHandler(ISkillLinkDbContext context, IEmbeddingService embeddingService)
        {
            _context = context;
            _embeddingService = embeddingService;
        }

        public async Task<int> Handle(GenerateSkillEmbeddingsCommand request, CancellationToken cancellationToken)
        {
            var skillsWithoutEmbeddings = await _context.Skills
                .Include(s => s.Category)
                .Where(s => string.IsNullOrEmpty(s.EmbeddingJson))
                .ToListAsync(cancellationToken);

            int count = 0;
            foreach (var skill in skillsWithoutEmbeddings)
            {
                var textToEmbed = $"Title: {skill.Title}. Category: {skill.Category.Name}. Description: {skill.Description}";
                
                var embedding = await _embeddingService.GenerateEmbeddingAsync(textToEmbed, cancellationToken);
                
                skill.EmbeddingJson = JsonSerializer.Serialize(embedding);
                count++;
            }

            if (count > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return count;
        }
    }
}
