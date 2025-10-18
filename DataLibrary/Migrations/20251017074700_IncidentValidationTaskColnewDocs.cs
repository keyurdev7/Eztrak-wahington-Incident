using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentValidationTaskColnewDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ComplateTime",
                table: "IncidentValidationTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "IncidentValidationTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "IncidentValidationTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "IncidentValidationTasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplateTime",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "IncidentValidationTasks");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "IncidentValidationTasks");
        }
    }
}
