namespace SkillLink.Application.UseCases.Auth.Commands.Login
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        int UserId
    );
}
