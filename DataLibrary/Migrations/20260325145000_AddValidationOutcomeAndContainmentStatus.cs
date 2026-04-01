using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    public partial class AddValidationOutcomeAndContainmentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidationDecision",
                table: "IncidentValidations",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "ContainmentStatus",
                table: "IncidentValidationGates",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationDecision",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "BasisForValidation",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "CriticalCustomerPresent",
                table: "IncidentValidations");

            migrationBuilder.DropColumn(
                name: "ContainmentStatus",
                table: "IncidentValidationGates");
        }
    }
}

