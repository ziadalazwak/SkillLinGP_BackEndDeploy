using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<AdminUserDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetAllUsersQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<AdminUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DomainUsers
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.Contains(request.Search)) ||
                    u.Email.Contains(request.Search));

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new AdminUserDto
                {
                    Id              = u.Id,
                    FullName        = u.FullName,
                    Email           = u.Email,
                    PhoneNumber     = u.PhoneNumber,
                    IsActive        = u.IsActive,
                    IsAdmin         = u.IsAdmin,
                    IsEmailVerified = u.IsEmailVerified,
                    CreditBalance   = u.CreditBalance,
                    CreatedAt       = u.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<AdminUserDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
