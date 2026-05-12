using MediatR;

namespace SkillLink.Application.UseCases.Trades.Commands.CreateTrade
{
    public record CreateTradeCommand : IRequest<int>
    {
        public int OffererId { get; init; }
        public int ReceiverId { get; init; }
        public int OfferedSkillId { get; init; }
        public int RequestedSkillId { get; init; }
        public string? Message { get; init; }
    }
}
