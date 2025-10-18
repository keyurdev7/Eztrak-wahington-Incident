using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentValidationsNewTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentValidationCommunicationHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientsIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageType = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationCommunicationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationCommunicationHistories_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncidentValidationPolicies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    PolicyId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<long>(type: "bigint", nullable: false),
                    TeamIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationPolicies_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncidentValidations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    ConfirmedSeverityLevelId = table.Column<long>(type: "bigint", nullable: false),
                    DiscoveryPerimeterId = table.Column<long>(type: "bigint", nullable: false),
                    AssignResponseTeams = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMarkFalseAlarm = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidations_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationCommunicationHistories_IncidentId",
                table: "IncidentValidationCommunicationHistories",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationPolicies_IncidentId",
                table: "IncidentValidationPolicies",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidations_IncidentId",
                table: "IncidentValidations",
                column: "IncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentValidationCommunicationHistories");

            migrationBuilder.DropTable(
                name: "IncidentValidationPolicies");

            migrationBuilder.DropTable(
                name: "IncidentValidations");
        }
    }
}
