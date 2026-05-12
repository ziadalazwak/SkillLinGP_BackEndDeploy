using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
    {
        private readonly ISkillLinkDbContext _context;

        public GetUserProfileQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .AsNoTracking()
                .Include(u => u.UserSkills.Where(us => !us.IsDeleted))
                    .ThenInclude(us => us.Skill)
                        .ThenInclude(s => s.Category)
                .Include(u => u.ReviewsReceived.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var skills = user.UserSkills
                .Where(us => !us.Skill.IsDeleted)
                .Select(us => new PublicUserSkillDto(
                    us.SkillId,
                    us.Skill.Title,
                    us.Skill.Category.Name,
                    us.Level.ToString(),
                    us.IsOffering
                ));

            double averageRating = 0;
            int reviewsCount = user.ReviewsReceived.Count;
            if (reviewsCount > 0)
            {
                averageRating = user.ReviewsReceived.Average(r => r.Rating);
            }

            return new UserProfileDto(
                user.Id,
                user.FullName ?? "Unknown User",
                user.Email,
                user.PhoneNumber,
                user.Bio,
                user.CreditBalance,
                user.ProfilePictureUrl,
                Math.Round(averageRating, 1),
                reviewsCount,
                user.IsAdmin,
                user.CreatedAt,
                skills
            );
        }
    }
}
