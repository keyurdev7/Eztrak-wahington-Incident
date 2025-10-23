using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class incidentphase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplateTime",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "IncidentValidationTasks");

            migrationBuilder.AddColumn<string>(
                name: "Phase",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Progress",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phase",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Incidents");

            migrationBuilder.AddColumn<DateTime>(
                name: "ComplateTime",
                table: "IncidentValidationTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "IncidentValidationTasks",
                type: "datetime2",
                nullable: true);
        }
    }
}
