using MediatR;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetPublicUserProfile
{
    public record GetPublicUserProfileQuery(int UserId) : IRequest<PublicUserProfileDto?>;
}
