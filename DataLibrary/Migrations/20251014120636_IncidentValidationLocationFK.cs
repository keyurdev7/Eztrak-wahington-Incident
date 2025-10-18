using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentValidationLocationFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdditionalLocationId",
                table: "IncidentValidationLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationLocations_AdditionalLocationId",
                table: "IncidentValidationLocations",
                column: "AdditionalLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentValidationLocations_AdditionalLocations_AdditionalLocationId",
                table: "IncidentValidationLocations",
                column: "AdditionalLocationId",
                principalTable: "AdditionalLocations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentValidationLocations_AdditionalLocations_AdditionalLocationId",
                table: "IncidentValidationLocations");

            migrationBuilder.DropIndex(
                name: "IX_IncidentValidationLocations_AdditionalLocationId",
                table: "IncidentValidationLocations");

            migrationBuilder.DropColumn(
                name: "AdditionalLocationId",
                table: "IncidentValidationLocations");
        }
    }
}
