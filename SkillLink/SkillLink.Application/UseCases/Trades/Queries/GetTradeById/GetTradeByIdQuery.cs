using MediatR;

namespace SkillLink.Application.UseCases.Trades.Queries.GetTradeById
{
    public record GetTradeByIdQuery(int Id, int UserId) : IRequest<SkillTradeDetailDto?>;
}
