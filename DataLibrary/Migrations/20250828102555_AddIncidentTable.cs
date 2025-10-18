using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelationshipId = table.Column<long>(type: "bigint", nullable: true),
                    EventTypeId = table.Column<long>(type: "bigint", nullable: true),
                    SeverityLevelId = table.Column<long>(type: "bigint", nullable: true),
                    StatusLegendId = table.Column<long>(type: "bigint", nullable: true),
                    CallerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Landmark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionIssue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GasPresentId = table.Column<long>(type: "bigint", nullable: true),
                    HissingPresentId = table.Column<long>(type: "bigint", nullable: true),
                    VisibleDamagePresentId = table.Column<long>(type: "bigint", nullable: true),
                    PeopleInjuredId = table.Column<long>(type: "bigint", nullable: true),
                    EvacuationRequiredId = table.Column<long>(type: "bigint", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportInfoNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Incidents_Relationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "Relationships",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Incidents_SeverityLevels_SeverityLevelId",
                        column: x => x.SeverityLevelId,
                        principalTable: "SeverityLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Incidents_StatusLegends_StatusLegendId",
                        column: x => x.StatusLegendId,
                        principalTable: "StatusLegends",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EventTypeId",
                table: "Incidents",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_RelationshipId",
                table: "Incidents",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_SeverityLevelId",
                table: "Incidents",
                column: "SeverityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_StatusLegendId",
                table: "Incidents",
                column: "StatusLegendId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}
