using Microsoft.EntityFrameworkCore.Storage;
using SkillLink.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace SkillLink.Infrastructure.Services
{
    public class OtpService: IOtpService
    {
        private readonly IDatabase _db;
        private readonly IEmailService _emailService;
        public OtpService(IConnectionMultiplexer redis, IEmailService emailService)
        {
            _db = redis.GetDatabase();
            _emailService = emailService;
        }

        public async Task SendOtpAsync(string email)
        {
            var otp = GenerateOtp();
            
            var key = $"otp:{email}";
            await _db.StringSetAsync(key, otp, TimeSpan.FromMinutes(5));

            await _emailService.SendOtpEmail(email, otp);

        }

        public async Task<bool> VerifyOtpAsync(string email, string code)
        {
            var key = $"otp:{email}";
            var storedOtp = await _db.StringGetAsync(key);

            if (storedOtp.IsNullOrEmpty)
                return false;

            if (storedOtp != code)
                return false;

            // ✅ delete after success
            await _db.KeyDeleteAsync(key);

            return true;
        }
        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
