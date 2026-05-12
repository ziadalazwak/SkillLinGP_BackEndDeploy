using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillLink.Application.Common;
using SkillLink.Application.Interfaces;

namespace SkillLink.Application.UseCases.Credits.Queries.GetCreditTransactions
{
    public class GetCreditTransactionsQueryHandler : IRequestHandler<GetCreditTransactionsQuery, PaginatedList<CreditTransactionDto>>
    {
        private readonly ISkillLinkDbContext _context;

        public GetCreditTransactionsQueryHandler(ISkillLinkDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<CreditTransactionDto>> Handle(GetCreditTransactionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CreditTransactions
                .Where(ct => ct.UserId == request.UserId)
                .AsQueryable();

            if (request.Type.HasValue)
            {
                query = query.Where(ct => ct.Type == request.Type.Value);
            }

            if (request.From.HasValue)
            {
                query = query.Where(ct => ct.CreatedAt >= request.From.Value);
            }

            if (request.To.HasValue)
            {
                query = query.Where(ct => ct.CreatedAt <= request.To.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(ct => ct.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ct => new CreditTransactionDto(
                    ct.Id,
                    ct.Amount,
                    ct.Type,
                    ct.BalanceAfter,
                    ct.SessionId,
                    ct.Description,
                    ct.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedList<CreditTransactionDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
