using MediatR;
using SkillLink.Application.UseCases.Users.DTOs;

namespace SkillLink.Application.UseCases.Users.Queries.GetUserProfile
{
    public record GetUserProfileQuery(int UserId) : IRequest<UserProfileDto>;
}
