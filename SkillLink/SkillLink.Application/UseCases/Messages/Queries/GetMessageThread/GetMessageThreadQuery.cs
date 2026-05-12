using MediatR;
using SkillLink.Application.Common;
using SkillLink.Application.UseCases.Messages.DTOs;

namespace SkillLink.Application.UseCases.Messages.Queries.GetMessageThread
{
    public record GetMessageThreadQuery(int CurrentUserId, int TargetUserId, int Page = 1, int PageSize = 30) 
        : IRequest<PaginatedList<MessageDto>>;
}
