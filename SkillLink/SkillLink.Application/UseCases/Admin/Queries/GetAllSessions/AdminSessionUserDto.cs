namespace SkillLink.Application.UseCases.Admin.Queries.GetAllSessions
{
    public class AdminSessionUserDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
