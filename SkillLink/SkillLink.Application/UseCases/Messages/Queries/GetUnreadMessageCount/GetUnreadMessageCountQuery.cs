using MediatR;

namespace SkillLink.Application.UseCases.Messages.Queries.GetUnreadMessageCount
{
    public record GetUnreadMessageCountQuery(int UserId) : IRequest<int>;
}
