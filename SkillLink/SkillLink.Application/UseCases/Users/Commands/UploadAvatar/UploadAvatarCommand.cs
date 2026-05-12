using MediatR;
using System.IO;

namespace SkillLink.Application.UseCases.Users.Commands.UploadAvatar
{
    public class UploadAvatarCommand : IRequest<string?>
    {
        public int UserId { get; set; }
        public Stream FileStream { get; set; } = Stream.Null;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
