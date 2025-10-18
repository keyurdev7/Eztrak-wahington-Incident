using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentValidationTaskFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IncidentId",
                table: "IncidentValidationTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncidentValidationId",
                table: "IncidentValidationTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationTasks_IncidentId",
                table: "IncidentValidationTasks",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationTasks_IncidentValidationId",
                table: "IncidentValidationTasks",
                column: "IncidentValidationId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentValidationTasks_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationTasks",
                column: "IncidentValidationId",
                principalTable: "IncidentValidations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentValidationTasks_Incidents_IncidentId",
                table: "IncidentValidationTasks",
                column: "IncidentId",
                principalTable: "Incidents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentValidationTasks_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentValidationTasks_Incidents_IncidentId",
                table: "IncidentValidationTasks");

            migrationBuilder.DropIndex(
                name: "IX_IncidentValidationTasks_IncidentId",
                table: "IncidentValidationTasks");

            migrationBuilder.DropIndex(
                name: "IX_IncidentValidationTasks_IncidentValidationId",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "IncidentId",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "IncidentValidationId",
                table: "IncidentValidationTasks");
        }
    }
}
