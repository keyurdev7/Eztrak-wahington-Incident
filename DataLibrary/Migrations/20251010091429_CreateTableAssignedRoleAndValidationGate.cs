using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableAssignedRoleAndValidationGate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentValidationAssignedRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: false),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: false),
                    IncidentCommander = table.Column<long>(type: "bigint", nullable: true),
                    FieldEnvRep = table.Column<long>(type: "bigint", nullable: true),
                    GEC_Coordinator = table.Column<long>(type: "bigint", nullable: true),
                    EngineeringLead = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationAssignedRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentValidationGates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: false),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: false),
                    ContainmentAcknowledgement = table.Column<bool>(type: "bit", nullable: true),
                    Exception = table.Column<bool>(type: "bit", nullable: true),
                    IndependentInspection = table.Column<bool>(type: "bit", nullable: true),
                    Regulatory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationGates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentValidationAssignedRoles");

            migrationBuilder.DropTable(
                name: "IncidentValidationGates");
        }
    }
}
