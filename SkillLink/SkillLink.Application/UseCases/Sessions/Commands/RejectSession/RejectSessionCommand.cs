using MediatR;

namespace SkillLink.Application.UseCases.Sessions.Commands.RejectSession
{
    public record RejectSessionCommand(int SessionId, int ProviderId, string? Reason) : IRequest<bool>;
}
