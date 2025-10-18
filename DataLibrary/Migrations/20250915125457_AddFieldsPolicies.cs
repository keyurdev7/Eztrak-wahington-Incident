using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PolicySteps",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "Policies",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_TeamId",
                table: "Policies",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_IncidentTeams_TeamId",
                table: "Policies",
                column: "TeamId",
                principalTable: "IncidentTeams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_IncidentTeams_TeamId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_TeamId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "PolicySteps",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Policies");
        }
    }
}
