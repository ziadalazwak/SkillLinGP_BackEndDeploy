using SkillLink.Domain.Models;

namespace SkillLink.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
