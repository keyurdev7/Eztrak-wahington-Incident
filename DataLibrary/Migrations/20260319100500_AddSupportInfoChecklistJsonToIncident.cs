using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    [Migration("20260319100500_AddSupportInfoChecklistJsonToIncident")]
    public partial class AddSupportInfoChecklistJsonToIncident : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportInfoChecklistJson",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportInfoChecklistJson",
                table: "Incidents");
        }
    }
}
