using SkillLink.Domain.Models;
using System.Security.Claims;

namespace SkillLink.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Success, string IdentityId, IEnumerable<string> Errors)> RegisterUserAsync(string email, string password, string fullName);
        Task<bool> CheckPasswordAsync(string identityId, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<(bool Success, IEnumerable<string> Errors)> VerifyEmailAsync(string identityId, string token);
        Task<(bool Success, string Token)> GenerateEmailConfirmationTokenAsync(string identityId);
        Task<(bool Success, string Token)> GeneratePasswordResetTokenAsync(string email);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
