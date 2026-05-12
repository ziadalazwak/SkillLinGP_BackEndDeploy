using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSkills
{
    public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, PaginatedList<AdminSkillDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetAllSkillsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<AdminSkillDto>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Skills
                .Include(s => s.Category)
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(s =>
                    s.Title.Contains(request.Search) ||
                    (s.Description != null && s.Description.Contains(request.Search)));

            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new AdminSkillDto
                {
                    Id           = s.Id,
                    Title        = s.Title,
                    Description  = s.Description,
                    CategoryId   = s.CategoryId,
                    CategoryName = s.Category.Name,
                    IsActive     = s.IsActive,
                    ImageUrl     = s.ImageUrl,
                    CreatedAt    = s.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<AdminSkillDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
