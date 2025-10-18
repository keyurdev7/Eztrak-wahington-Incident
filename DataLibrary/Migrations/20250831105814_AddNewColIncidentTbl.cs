using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColIncidentTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_EventTypes_EventTypeId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EventTypeId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Incidents");

            migrationBuilder.AddColumn<string>(
                name: "EventTypeIds",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSameCallerAddress",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OtherEventDetail",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OtherEventId",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTypeIds",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IsSameCallerAddress",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "OtherEventDetail",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "OtherEventId",
                table: "Incidents");

            migrationBuilder.AddColumn<long>(
                name: "EventTypeId",
                table: "Incidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EventTypeId",
                table: "Incidents",
                column: "EventTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_EventTypes_EventTypeId",
                table: "Incidents",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id");
        }
    }
}
