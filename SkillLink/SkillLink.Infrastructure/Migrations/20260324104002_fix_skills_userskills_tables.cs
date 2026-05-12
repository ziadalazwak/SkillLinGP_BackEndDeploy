using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix_skills_userskills_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransaction_DomainUsers_UserId",
                table: "CreditTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransaction_Sessions_SessionId",
                table: "CreditTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkill_DomainUsers_UserId",
                table: "UserSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkill_Skills_SkillId",
                table: "UserSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkill",
                table: "UserSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditTransaction",
                table: "CreditTransaction");

            migrationBuilder.DropColumn(
                name: "CreditCost",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "ExchangeMode",
                table: "Skills");

            migrationBuilder.RenameTable(
                name: "UserSkill",
                newName: "UserSkills");

            migrationBuilder.RenameTable(
                name: "CreditTransaction",
                newName: "CreditTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkill_UserId_SkillId",
                table: "UserSkills",
                newName: "IX_UserSkills_UserId_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkill_SkillId",
                table: "UserSkills",
                newName: "IX_UserSkills_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditTransaction_UserId",
                table: "CreditTransactions",
                newName: "IX_CreditTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditTransaction_SessionId",
                table: "CreditTransactions",
                newName: "IX_CreditTransactions_SessionId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "UserSkills",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditCost",
                table: "UserSkills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMode",
                table: "UserSkills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "UserSkills",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExchangeMode",
                table: "UserSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditTransactions",
                table: "CreditTransactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransactions_DomainUsers_UserId",
                table: "CreditTransactions",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransactions_Sessions_SessionId",
                table: "CreditTransactions",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_DomainUsers_UserId",
                table: "UserSkills",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransactions_DomainUsers_UserId",
                table: "CreditTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransactions_Sessions_SessionId",
                table: "CreditTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_DomainUsers_UserId",
                table: "UserSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditTransactions",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "CreditCost",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "UserSkills");

            migrationBuilder.DropColumn(
                name: "ExchangeMode",
                table: "UserSkills");

            migrationBuilder.RenameTable(
                name: "UserSkills",
                newName: "UserSkill");

            migrationBuilder.RenameTable(
                name: "CreditTransactions",
                newName: "CreditTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkills_UserId_SkillId",
                table: "UserSkill",
                newName: "IX_UserSkill_UserId_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkills_SkillId",
                table: "UserSkill",
                newName: "IX_UserSkill_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditTransactions_UserId",
                table: "CreditTransaction",
                newName: "IX_CreditTransaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditTransactions_SessionId",
                table: "CreditTransaction",
                newName: "IX_CreditTransaction_SessionId");

            migrationBuilder.AddColumn<decimal>(
                name: "CreditCost",
                table: "Skills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMode",
                table: "Skills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExchangeMode",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "UserSkill",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkill",
                table: "UserSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditTransaction",
                table: "CreditTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransaction_DomainUsers_UserId",
                table: "CreditTransaction",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransaction_Sessions_SessionId",
                table: "CreditTransaction",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_DomainUsers_UserId",
                table: "UserSkill",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_Skills_SkillId",
                table: "UserSkill",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
