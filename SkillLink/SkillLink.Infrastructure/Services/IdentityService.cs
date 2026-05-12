using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SkillLinkDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SkillLinkDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<(bool Success, string IdentityId, IEnumerable<string> Errors)> RegisterUserAsync(string email, string password, string fullName)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = email,
                Email = email, // This will be set after the domain user is created
            };

            var result = await _userManager.CreateAsync(applicationUser, password);

            if (!result.Succeeded)
            {
                return (false, string.Empty, result.Errors.Select(e => e.Description));
            }

            return (true, applicationUser.Id, Array.Empty<string>());
        }

        public async Task<bool> CheckPasswordAsync(string identityId, string password)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            if (user == null) return false;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.DomainUsers.SingleOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> VerifyEmailAsync(string identityId, string token)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return (false, result.Errors.Select(e => e.Description));

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string Token)> GenerateEmailConfirmationTokenAsync(string identityId)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            if (user == null) return (false, string.Empty);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return (true, token);
        }

        public async Task<(bool Success, string Token)> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, string.Empty);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return (true, token);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, new[] { "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded) return (false, result.Errors.Select(e => e.Description));

            return (true, Array.Empty<string>());
        }
    }
}
