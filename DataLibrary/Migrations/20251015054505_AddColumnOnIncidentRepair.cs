using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnOnIncidentRepair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PFO_Path",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PFO_Remark",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SOL_Path",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SOL_Remark",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VTF_Path",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VTF_Remark",
                table: "IncidentValidationRepairs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PFO_Path",
                table: "IncidentValidationRepairs");

            migrationBuilder.DropColumn(
                name: "PFO_Remark",
                table: "IncidentValidationRepairs");

            migrationBuilder.DropColumn(
                name: "SOL_Path",
                table: "IncidentValidationRepairs");

            migrationBuilder.DropColumn(
                name: "SOL_Remark",
                table: "IncidentValidationRepairs");

            migrationBuilder.DropColumn(
                name: "VTF_Path",
                table: "IncidentValidationRepairs");

            migrationBuilder.DropColumn(
                name: "VTF_Remark",
                table: "IncidentValidationRepairs");
        }
    }
}
