using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class modifying_StreetServiceRequest_dbmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrainageDescription",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "OtherDescription",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "ParkwayTreeDescription",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "PotholesDescription",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "SideWalkDescription",
                table: "StreetServiceRequests");

            migrationBuilder.RenameColumn(
                name: "StreetSweepingDescription",
                table: "StreetServiceRequests",
                newName: "Description");

            migrationBuilder.AddColumn<bool>(
                name: "Drainage",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Other",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ParkwayTree",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Potholes",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SideWalk",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StreetSweeping",
                table: "StreetServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Drainage",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "Other",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "ParkwayTree",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "Potholes",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "SideWalk",
                table: "StreetServiceRequests");

            migrationBuilder.DropColumn(
                name: "StreetSweeping",
                table: "StreetServiceRequests");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "StreetServiceRequests",
                newName: "StreetSweepingDescription");

            migrationBuilder.AddColumn<string>(
                name: "DrainageDescription",
                table: "StreetServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherDescription",
                table: "StreetServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParkwayTreeDescription",
                table: "StreetServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PotholesDescription",
                table: "StreetServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SideWalkDescription",
                table: "StreetServiceRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
