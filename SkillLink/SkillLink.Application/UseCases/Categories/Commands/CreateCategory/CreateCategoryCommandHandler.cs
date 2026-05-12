using MediatR;
using SkillLink.Application.Interfaces;
using SkillLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ISkillLinkDbContext _context;

        public CreateCategoryCommandHandler(ISkillLinkDbContext context) { _context=context; }
        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCategory = new SkillCategory
            {
                Name = request.Name??"",
                Description = request.Description,
                IconUrl = request.IconUrl,
                ImageUrl = request.ImageUrl
            };

            await _context.SkillCategories.AddAsync(newCategory, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return newCategory.Id;
        }
    }
}
