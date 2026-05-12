using MediatR;

namespace SkillLink.Application.UseCases.Trades.Commands.CancelTrade
{
    public record CancelTradeCommand(int TradeId, int UserId) : IRequest<bool>;
}
