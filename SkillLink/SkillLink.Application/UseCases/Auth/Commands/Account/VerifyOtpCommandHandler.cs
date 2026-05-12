using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Auth.Commands.Account
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, bool>
    {
        private readonly IOtpService _otpService;
        private readonly ISkillLinkDbContext _context;

        public VerifyOtpCommandHandler(IOtpService otpService, ISkillLinkDbContext context)
        {
            _otpService = otpService;
            _context = context;
        }

        public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _otpService.VerifyOtpAsync(request.Email, request.Otp);
            if (!isValid)
                throw new Exception("Invalid or expired OTP.");

            // Mark the user as email-verified
            var user = await _context.DomainUsers
                .SingleOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

            if (user == null)
                throw new Exception("User not found.");

            user.IsEmailVerified = true;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
