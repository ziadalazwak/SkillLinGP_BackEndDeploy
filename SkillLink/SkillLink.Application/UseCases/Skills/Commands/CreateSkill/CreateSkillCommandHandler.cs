using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Skills.Commands.CreateSkill
{


   
public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, int>
    {
        private readonly ISkillLinkDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public CreateSkillHandler(ISkillLinkDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
            
        }

        public async Task<int> Handle(CreateSkillCommand request, CancellationToken ct)
        {
            // 1. Verify the category exists
            //var categoryExists = await _context.SkillCategories
            //    .AnyAsync(c => c.Id == request.CategoryId && c.IsActive, ct);

            //if (!categoryExists)
            //    return 0;

            // 2. Verify the provider exists
            //var providerExists = await _context.DomainUsers
            //    .AnyAsync(u => u.Id == request.ProviderId, ct);

            //if (!providerExists)
            //    return 0;

            // 3. Upload image if provided
            string? imageUrl = null;

            if (request.ImageBytes is not null && request.ImageExtension is not null)
            {
                using var stream = new MemoryStream(request.ImageBytes);

                imageUrl = await _fileStorage.SaveAsync(
                    fileStream: stream,
                    fileName: $"skill{request.ImageExtension}",
                    folder: "skills",
                    ct: ct);
            }

            // 4. Create via factory — handler does not set defaults
            var skill = Skill.Create(
                request.Title,
                request.Description,
                request.CategoryId,
                imageUrl);
            
            skill.IsActive=true;
            await _context.Skills.AddAsync(skill, ct);
            // Save immediately so skill.Id is generated
            await _context.SaveChangesAsync(ct);


            return skill.Id;
        }
    }
}

