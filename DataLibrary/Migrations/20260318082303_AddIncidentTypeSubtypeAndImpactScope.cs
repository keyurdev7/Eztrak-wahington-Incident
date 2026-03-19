using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentTypeSubtypeAndImpactScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EventSubTypeId",
                table: "Incidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EventTypeId",
                table: "Incidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpactScope",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventSubTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSubTypes_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EventSubTypeId",
                table: "Incidents",
                column: "EventSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EventTypeId",
                table: "Incidents",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubTypes_EventTypeId",
                table: "EventSubTypes",
                column: "EventTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_EventSubTypes_EventSubTypeId",
                table: "Incidents",
                column: "EventSubTypeId",
                principalTable: "EventSubTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_EventTypes_EventTypeId",
                table: "Incidents",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_EventSubTypes_EventSubTypeId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_EventTypes_EventTypeId",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "EventSubTypes");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EventSubTypeId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EventTypeId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EventSubTypeId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ImpactScope",
                table: "Incidents");
        }
    }
}
