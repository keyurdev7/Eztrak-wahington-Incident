using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasisForValidation",
                table: "IncidentValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CriticalCustomerPresent",
                table: "IncidentValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationDecision",
                table: "IncidentValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContainmentStatus",
                table: "IncidentValidationGates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasisForValidation",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "CriticalCustomerPresent",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "ValidationDecision",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "ContainmentStatus",
                table: "IncidentValidationGates");
        }
    }
}
