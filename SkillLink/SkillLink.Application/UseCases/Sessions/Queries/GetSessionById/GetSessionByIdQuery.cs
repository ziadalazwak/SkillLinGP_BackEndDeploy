using MediatR;

namespace SkillLink.Application.UseCases.Sessions.Queries.GetSessionById
{
    public record GetSessionByIdQuery(int Id, int UserId) : IRequest<SessionDetailDto?>;
}
