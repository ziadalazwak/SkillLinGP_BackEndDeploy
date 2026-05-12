using MediatR;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Queries.GetConversations
{
    public record GetConversationsQuery(int UserId) : IRequest<IEnumerable<ConversationSummaryDto>>;
}
