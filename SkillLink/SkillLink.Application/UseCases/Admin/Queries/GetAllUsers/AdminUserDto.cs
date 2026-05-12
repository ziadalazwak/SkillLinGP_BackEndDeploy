namespace SkillLink.Application.UseCases.Admin.Queries.GetAllUsers
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmailVerified { get; set; }
        public int CreditBalance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
