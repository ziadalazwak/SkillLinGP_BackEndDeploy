using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Users.Commands.UploadAvatar
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string?>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public UploadAvatarCommandHandler(ISkillLinkDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<string?> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.DomainUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
            if (user == null)
            {
                return null;
            }

            // Remove existing avatar if we have one
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                await _fileStorageService.DeleteAsync(user.ProfilePictureUrl, cancellationToken);
            }

            var fileUrl = await _fileStorageService.SaveAsync(request.FileStream, request.FileName, "avatars", cancellationToken);

            user.ProfilePictureUrl = fileUrl;
            await _context.SaveChangesAsync(cancellationToken);

            return fileUrl;
        }
    }
}
