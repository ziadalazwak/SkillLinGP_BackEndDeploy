using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillLink.Application.UseCases.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler
        : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetCategoriesQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.SkillCategories.AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .Select(c => new CategoryDto
                {
                    Id          = c.Id,
                    Name        = c.Name,
                    Description = c.Description,
                    IconUrl     = c.IconUrl,
                    ImageUrl    = c.ImageUrl
                })
                .ToListAsync(cancellationToken);
        }
    }
}
