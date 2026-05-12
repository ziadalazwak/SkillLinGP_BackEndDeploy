using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SkillLink.Infrastructure
{
    /// <summary>
    /// Allows EF Core design-time tools (dotnet ef migrations add/update) to
    /// create a <see cref="SkillLinkDbContext"/> without starting the full host.
    /// </summary>
    public class SkillLinkDbContextFactory : IDesignTimeDbContextFactory<SkillLinkDbContext>
    {
        public SkillLinkDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SkillLinkDbContext>();

            // Use the same connection string that is in appsettings.json.
            // This is only used at design time; the running app uses DI.
            optionsBuilder.UseSqlServer(
                "Server=db51427.public.databaseasp.net; Database=db51427; User Id=db51427; Password=8Zz-J@2b?dE6; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;");

            return new SkillLinkDbContext(optionsBuilder.Options);
        }
    }
}
