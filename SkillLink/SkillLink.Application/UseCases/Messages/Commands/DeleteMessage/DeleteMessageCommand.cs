using MediatR;

namespace SkillLink.Application.UseCases.Messages.Commands.DeleteMessage
{
    public record DeleteMessageCommand(int MessageId, int CurrentUserId) : IRequest;
}
