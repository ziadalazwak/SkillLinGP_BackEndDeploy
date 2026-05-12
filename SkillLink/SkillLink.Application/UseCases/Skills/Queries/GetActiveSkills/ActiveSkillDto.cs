namespace SkillLink.Application.UseCases.Skills.Queries.GetActiveSkills
{
    public record ActiveSkillDto(
        int Id,
        string Title,
        string CategoryName,
        string Description,
        string? ImageUrl
    );
}
