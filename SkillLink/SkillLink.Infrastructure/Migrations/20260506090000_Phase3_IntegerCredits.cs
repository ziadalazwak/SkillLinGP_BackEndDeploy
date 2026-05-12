using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_IntegerCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sessions.CreditCost: decimal? → int?
            migrationBuilder.AlterColumn<int>(
                name: "CreditCost",
                table: "Sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            // Users.CreditBalance: decimal → int
            migrationBuilder.AlterColumn<int>(
                name: "CreditBalance",
                table: "DomainUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: false);

            // Users.HeldCreditBalance: decimal → int
            migrationBuilder.AlterColumn<int>(
                name: "HeldCreditBalance",
                table: "DomainUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: false);

            // CreditTransactions.Amount: decimal → int
            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "CreditTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: false);

            // CreditTransactions.BalanceAfter: decimal → int
            migrationBuilder.AlterColumn<int>(
                name: "BalanceAfter",
                table: "CreditTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CreditCost",
                table: "Sessions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditBalance",
                table: "DomainUsers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "HeldCreditBalance",
                table: "DomainUsers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "CreditTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceAfter",
                table: "CreditTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
