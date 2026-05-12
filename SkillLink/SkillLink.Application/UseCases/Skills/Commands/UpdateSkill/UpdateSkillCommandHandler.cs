using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, bool>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public UpdateSkillCommandHandler(ISkillLinkDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<bool> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

            if (skill == null)
            {
                return false;
            }

            // removed ownership checks because skills are a global dictionary

            if (request.Title != null) skill.Title = request.Title;
            if (request.Description != null) skill.Description = request.Description;
            if (request.IsActive.HasValue) skill.IsActive = request.IsActive.Value;

            if (request.ImageBytes is not null && request.ImageExtension is not null)
            {
                using var stream = new MemoryStream(request.ImageBytes);
                skill.ImageUrl = await _fileStorage.SaveAsync(
                    fileStream: stream,
                    fileName: $"skill{request.ImageExtension}",
                    folder: "skills",
                    ct: cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
