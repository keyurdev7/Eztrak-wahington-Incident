using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    [Migration("20260319094500_AddDescriptionChecklistJsonToIncident")]
    public partial class AddDescriptionChecklistJsonToIncident : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionChecklistJson",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionChecklistJson",
                table: "Incidents");
        }
    }
}
