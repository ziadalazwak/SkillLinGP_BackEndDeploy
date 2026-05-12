using MediatR;

namespace SkillLink.Application.UseCases.Sessions.Commands.CancelSession
{
    public record CancelSessionCommand(int SessionId, int UserId, string? Reason) : IRequest<bool>;
}
