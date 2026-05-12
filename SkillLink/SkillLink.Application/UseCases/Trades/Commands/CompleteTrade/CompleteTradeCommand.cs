using MediatR;

namespace SkillLink.Application.UseCases.Trades.Commands.CompleteTrade
{
    public record CompleteTradeCommand(int TradeId, int UserId) : IRequest<bool>;
}
