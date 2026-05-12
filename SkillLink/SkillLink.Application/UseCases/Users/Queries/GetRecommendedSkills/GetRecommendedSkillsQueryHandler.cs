using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Users.DTOs;
using System.Text.Json;

namespace SkillLink.Application.UseCases.Users.Queries.GetRecommendedSkills
{
    public class GetRecommendedSkillsQueryHandler : IRequestHandler<GetRecommendedSkillsQuery, IEnumerable<RecommendedSkillDto>>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IEmbeddingService _embeddingService;

        public GetRecommendedSkillsQueryHandler(ISkillLinkDbContext context, IEmbeddingService embeddingService)
        {
            _context = context;
            _embeddingService = embeddingService;
        }

        public async Task<IEnumerable<RecommendedSkillDto>> Handle(GetRecommendedSkillsQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .AsNoTracking()
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null) return Enumerable.Empty<RecommendedSkillDto>();

            var userSkillIds = user.UserSkills.Select(us => us.SkillId).ToHashSet();

            var candidateUserSkills = await _context.UserSkills
                .AsNoTracking()
                .Include(us => us.Skill)
                    .ThenInclude(s => s.Category)
                .Include(us => us.User)
                .Where(us => us.IsOffering && us.Skill.IsActive)
                .Where(us => !string.IsNullOrEmpty(us.Skill.EmbeddingJson))
                .Where(us => !userSkillIds.Contains(us.SkillId))
                .Where(us => us.UserId != request.UserId)
                .Where(us=>us.User.IsActive==true)
                .ToListAsync(cancellationToken);

            if (!candidateUserSkills.Any())
            {
                return Enumerable.Empty<RecommendedSkillDto>();
            }

            // Create context string regarding the user
            var userContextParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(user.Bio))
            {
                userContextParts.Add($"Bio: {user.Bio}");
            }

            if (user.UserSkills.Any())
            {
                var skillNames = string.Join(", ", user.UserSkills.Select(us => us.Skill.Title));
                userContextParts.Add($"Current skills: {skillNames}");
            }

            // Fallback for brand new users with no info
            if (!userContextParts.Any())
            {
                return candidateUserSkills.Take(request.TopN).Select(us => new RecommendedSkillDto(
                    us.Skill.Id,
                    us.Id,
                    us.Skill.Title, 
                    us.Skill.Category.Name, 
                    us.Skill.Description, 
                    us.Skill.ImageUrl, 
                    0.0,
                    us.User.FullName ?? "Unknown User",
                    us.UserId,
                    us.DurationMinutes.HasValue ? (int)Math.Round(us.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero) : (int?)null,
                    us.DurationMinutes??0,
                    us.ExchangeMode,
                    us.Level
                ));
            }

            var userContext = string.Join(". ", userContextParts);
            var userVector = await _embeddingService.GenerateEmbeddingAsync(userContext, cancellationToken);

            var rankedSkills = new List<(Domain.Models.UserSkill UserSkill, double Score)>();

            foreach (var us in candidateUserSkills)
            {
                var skillVector = JsonSerializer.Deserialize<float[]>(us.Skill.EmbeddingJson!);
                if (skillVector == null) continue;

                var score = ComputeCosineSimilarity(userVector, skillVector);
                rankedSkills.Add((us, score));
            }

            return rankedSkills
                .OrderByDescending(x => x.Score)
                .Take(request.TopN)
                .Select(x => new RecommendedSkillDto(
                    x.UserSkill.Skill.Id,
                    x.UserSkill.Id,
                    x.UserSkill.Skill.Title,
                    x.UserSkill.Skill.Category.Name,
                    x.UserSkill.Skill.Description,
                    x.UserSkill.Skill.ImageUrl,
                    Math.Round(x.Score, 3),
                    x.UserSkill.User.FullName ?? "Unknown User",
                    x.UserSkill.UserId,
                    x.UserSkill.DurationMinutes.HasValue ? (int)Math.Round(x.UserSkill.DurationMinutes.Value / 60m, MidpointRounding.AwayFromZero) : (int?)null,
                    x.UserSkill.DurationMinutes,
                    x.UserSkill.ExchangeMode,
                    x.UserSkill.Level
                ));
        }
       
        private double ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length) return 0;

            double dotProduct = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += vectorA[i] * vectorA[i];
                normB += vectorB[i] * vectorB[i];
            }

            if (normA == 0 || normB == 0) return 0;

            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}
