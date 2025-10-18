using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class NewTblIncidentValidationLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentValidationLocations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: true),
                    ConfirmedSeverityLevelId = table.Column<long>(type: "bigint", nullable: true),
                    DiscoveryPerimeterId = table.Column<long>(type: "bigint", nullable: true),
                    ICPLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationLocations_IncidentValidations_IncidentValidationId",
                        column: x => x.IncidentValidationId,
                        principalTable: "IncidentValidations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentValidationLocations_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationLocations_IncidentId",
                table: "IncidentValidationLocations",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationLocations_IncidentValidationId",
                table: "IncidentValidationLocations",
                column: "IncidentValidationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentValidationLocations");
        }
    }
}
