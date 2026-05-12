using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Commands.UpdateCategory
{

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ISkillLinkDbContext _context;

        public UpdateCategoryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.SkillCategories
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (category == null)
                throw new Exception ("Category not found");

            // Update only if values are provided
            if (request.Name is not null) category.Name = request.Name;
            if (request.Description is not null) category.Description = request.Description;
            if (request.IsActive.HasValue) category.IsActive = request.IsActive.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return new CategoryDto(category.Id, category.Name, category.Description, category.IsActive);
        }
    }
}
