using MediatR;

namespace SkillLink.Application.UseCases.Sessions.Commands.AcceptSession
{
    public record AcceptSessionCommand(int SessionId, int ProviderId) : IRequest<bool>;
}
