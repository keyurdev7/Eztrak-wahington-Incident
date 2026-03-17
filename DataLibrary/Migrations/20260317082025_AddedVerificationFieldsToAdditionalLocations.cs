using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddedVerificationFieldsToAdditionalLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportBatchId",
                table: "AdditionalLocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerificationPoint",
                table: "AdditionalLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNotes",
                table: "AdditionalLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationPhotoUrl",
                table: "AdditionalLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "AdditionalLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VerifiedByUserId",
                table: "AdditionalLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedByUserName",
                table: "AdditionalLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedOn",
                table: "AdditionalLocations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportBatchId",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "IsVerificationPoint",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerificationNotes",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerificationPhotoUrl",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserId",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserName",
                table: "AdditionalLocations");

            migrationBuilder.DropColumn(
                name: "VerifiedOn",
                table: "AdditionalLocations");
        }
    }
}
