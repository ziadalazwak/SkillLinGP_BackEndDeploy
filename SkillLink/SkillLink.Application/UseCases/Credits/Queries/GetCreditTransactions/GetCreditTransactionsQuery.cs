using MediatR;
using SkillLink.Application.Common;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Credits.Queries.GetCreditTransactions
{
    public record CreditTransactionDto(
        int Id,
        int Amount,
        TransactionType Type,
        int BalanceAfter,
        int? SessionId,
        string? Description,
        DateTime CreatedAt
    );

    public record GetCreditTransactionsQuery : IRequest<PaginatedList<CreditTransactionDto>>
    {
        public int UserId { get; init; }
        public TransactionType? Type { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
