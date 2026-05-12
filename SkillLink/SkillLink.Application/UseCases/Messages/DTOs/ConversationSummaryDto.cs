namespace SkillLink.Application.UseCases.Messages.DTOs
{
    public record ConversationSummaryDto(
        int TargetUserId,
        string TargetUserName,
        string? TargetUserProfilePictureUrl,
        string LastMessagePreview,
        DateTime LastMessageDate,
        int UnreadCount
    );
}
