namespace SkillLink.Application.UseCases.Messages.DTOs
{
    public record MessageDto(
        int Id,
        int SenderId,
        int ReceiverId,
        int? SessionId,
        string Content,
        string? AttachmentUrl,
        bool IsRead,
        DateTime SentAt,
        DateTime? ReadAt
    );
}
