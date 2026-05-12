using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_HourBasedCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename CreditCost → CreditCostPerHour (preserves existing data)
            migrationBuilder.RenameColumn(
                name: "CreditCost",
                table: "UserSkills",
                newName: "CreditCostPerHour");

            // Correct the precision on the renamed column
            migrationBuilder.AlterColumn<decimal>(
                name: "CreditCostPerHour",
                table: "UserSkills",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HeldCreditBalance",
                table: "DomainUsers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeldCreditBalance",
                table: "DomainUsers");

            migrationBuilder.RenameColumn(
                name: "CreditCostPerHour",
                table: "UserSkills",
                newName: "CreditCost");
        }
    }
}
