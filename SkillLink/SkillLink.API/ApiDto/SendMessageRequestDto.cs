namespace SkillLink.API.ApiDto
{
    public class SendMessageRequestDto
    {
        public int ReceiverId { get; set; }
        public int? SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public IFormFile? AttachmentFile { get; set; }
    }
}
