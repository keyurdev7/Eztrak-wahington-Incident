using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentValidationTaskCloseOutDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ComplateTime",
                table: "ValidationCloseouts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "ValidationCloseouts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ValidationCloseouts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ValidationCloseouts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplateTime",
                table: "ValidationCloseouts");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "ValidationCloseouts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ValidationCloseouts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ValidationCloseouts");
        }
    }
}
