using MediatR;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly IIdentityService _identityService;
        private readonly ISkillLinkDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOtpService _otpService;

        public RegisterUserCommandHandler(
            IIdentityService identityService,
            ISkillLinkDbContext context,
            IFileStorageService fileStorageService,
            IOtpService otpService)
        {
            _identityService = identityService;
            _context = context;
            _fileStorageService = fileStorageService;
            _otpService = otpService;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var (success, identityId, errors) = await _identityService.RegisterUserAsync(request.Email, request.Password, request.FullName);
            if (!success)
            {
                throw new InvalidOperationException($"Registration failed: {string.Join(", ", errors)}");
            }

            var domainUser = new User
            {
                IdentityId = identityId,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                IsEmailVerified = false,
                IsAdmin = request.IsAdmin
            };

            // Upload profile image if provided
            if (request.ProfileImageStream != null && request.ProfileImageFileName != null)
            {
                var imageUrl = await _fileStorageService.SaveAsync(
                    request.ProfileImageStream,
                    request.ProfileImageFileName,
                    "avatars",
                    cancellationToken);
                domainUser.ProfilePictureUrl = imageUrl;
            }

            // FR1.1 & FR3.1: New user welcome credits
            domainUser.AdjustCreditBalance(3);

            var initialTransaction = new CreditTransaction
            {
                Type = TransactionType.InitialBonus,
                Amount =3,
                BalanceAfter = 3,
                Description = "Welcome Bonus on Registration",
                UserId = domainUser.Id
            };

            domainUser.CreditTransactions.Add(initialTransaction);

            await _context.DomainUsers.AddAsync(domainUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Send OTP for email verification
            await _otpService.SendOtpAsync(request.Email);

            return domainUser.Id;
        }
    }
}
