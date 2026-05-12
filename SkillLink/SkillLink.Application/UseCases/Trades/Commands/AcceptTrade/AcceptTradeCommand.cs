using MediatR;

namespace SkillLink.Application.UseCases.Trades.Commands.AcceptTrade
{
    public record AcceptTradeCommand(int TradeId, int ReceiverId) : IRequest<bool>;
}
