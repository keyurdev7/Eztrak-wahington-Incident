using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class modifying_TaskLabor_dbmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "TaskLabors");

            migrationBuilder.AddColumn<long>(
                name: "CraftSkillId",
                table: "TaskLabors",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabors_CraftSkillId",
                table: "TaskLabors",
                column: "CraftSkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLabors_CraftSkills_CraftSkillId",
                table: "TaskLabors",
                column: "CraftSkillId",
                principalTable: "CraftSkills",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLabors_CraftSkills_CraftSkillId",
                table: "TaskLabors");

            migrationBuilder.DropIndex(
                name: "IX_TaskLabors_CraftSkillId",
                table: "TaskLabors");

            migrationBuilder.DropColumn(
                name: "CraftSkillId",
                table: "TaskLabors");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TaskLabors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
