using MediatR;

namespace SkillLink.Application.UseCases.Sessions.Commands.CompleteSession
{
    public record CompleteSessionCommand(int SessionId, int UserId) : IRequest<bool>;
}
