using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public UpdateUserProfileCommandHandler(ISkillLinkDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return false;
            }

            if (request.FullName != null) user.FullName = request.FullName;
            if (request.Bio != null) user.Bio = request.Bio;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;

            // Handle profile image upload
            if (request.ProfileImageStream != null && request.ProfileImageFileName != null)
            {
                // Delete old avatar if one exists
                if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
                    await _fileStorageService.DeleteAsync(user.ProfilePictureUrl, cancellationToken);

                var imageUrl = await _fileStorageService.SaveAsync(
                    request.ProfileImageStream,
                    request.ProfileImageFileName,
                    "avatars",
                    cancellationToken);
                user.ProfilePictureUrl = imageUrl;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
