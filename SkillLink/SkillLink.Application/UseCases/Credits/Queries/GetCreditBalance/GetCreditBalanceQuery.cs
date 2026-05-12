using MediatR;

namespace SkillLink.Application.UseCases.Credits.Queries.GetCreditBalance
{
    public record GetCreditBalanceQuery(int UserId) : IRequest<int>;
}
