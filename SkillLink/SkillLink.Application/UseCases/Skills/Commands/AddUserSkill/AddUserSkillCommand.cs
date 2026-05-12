using MediatR;
using SkillLink.Domain.Models;

namespace SkillLink.Application.UseCases.Skills.Commands.AddUserSkill
{
    public record AddUserSkillCommand(
        int UserId,
        int SkillId,
        SkillLevel Level,
        ExchangeMode ExchangeMode,
        int? DurationMinutes,
        string? DeliveryMode,
        string? Notes
    ) : IRequest<bool>;
}
