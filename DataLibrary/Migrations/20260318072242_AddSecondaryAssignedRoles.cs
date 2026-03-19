using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddSecondaryAssignedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EngineeringLeadSecondary",
                table: "IncidentValidationAssignedRoles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FieldEnvRepSecondary",
                table: "IncidentValidationAssignedRoles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GEC_CoordinatorSecondary",
                table: "IncidentValidationAssignedRoles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncidentCommanderSecondary",
                table: "IncidentValidationAssignedRoles",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineeringLeadSecondary",
                table: "IncidentValidationAssignedRoles");

            migrationBuilder.DropColumn(
                name: "FieldEnvRepSecondary",
                table: "IncidentValidationAssignedRoles");

            migrationBuilder.DropColumn(
                name: "GEC_CoordinatorSecondary",
                table: "IncidentValidationAssignedRoles");

            migrationBuilder.DropColumn(
                name: "IncidentCommanderSecondary",
                table: "IncidentValidationAssignedRoles");
        }
    }
}
