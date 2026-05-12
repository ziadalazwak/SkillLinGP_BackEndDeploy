using MediatR;

namespace SkillLink.Application.UseCases.Trades.Commands.RejectTrade
{
    public record RejectTradeCommand(int TradeId, int ReceiverId) : IRequest<bool>;
}
