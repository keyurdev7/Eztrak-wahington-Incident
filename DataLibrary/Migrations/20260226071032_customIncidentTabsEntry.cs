using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class customIncidentTabsEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentValidationAssessments");

            migrationBuilder.DropTable(
                name: "IncidentValidationRepairs");

            migrationBuilder.CreateTable(
                name: "IncidentValidationAssessmentTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: true),
                    RoleIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationAssessmentTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationAssessmentTasks_IncidentValidations_IncidentValidationId",
                        column: x => x.IncidentValidationId,
                        principalTable: "IncidentValidations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentValidationAssessmentTasks_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncidentValidationRepairTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: true),
                    RoleIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationRepairTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationRepairTasks_IncidentValidations_IncidentValidationId",
                        column: x => x.IncidentValidationId,
                        principalTable: "IncidentValidations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentValidationRepairTasks_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationAssessmentTasks_IncidentId",
                table: "IncidentValidationAssessmentTasks",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationAssessmentTasks_IncidentValidationId",
                table: "IncidentValidationAssessmentTasks",
                column: "IncidentValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationRepairTasks_IncidentId",
                table: "IncidentValidationRepairTasks",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationRepairTasks_IncidentValidationId",
                table: "IncidentValidationRepairTasks",
                column: "IncidentValidationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentValidationAssessmentTasks");

            migrationBuilder.DropTable(
                name: "IncidentValidationRepairTasks");

            migrationBuilder.CreateTable(
                name: "IncidentValidationAssessments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<long>(type: "bigint", nullable: true),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: true),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EGEC_ICT_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    EGEC_ICT_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_ICT_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_ICT_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_ICT_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_ICT_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    EGEC_MLP_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    EGEC_MLP_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_MLP_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_MLP_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_MLP_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_MLP_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    EGEC_RSM_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    EGEC_RSM_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_RSM_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_RSM_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EGEC_RSM_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EGEC_RSM_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    FER_LC_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    FER_LC_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FER_LC_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FER_LC_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FER_LC_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FER_LC_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    FER_PCA_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    FER_PCA_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FER_PCA_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FER_PCA_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FER_PCA_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FER_PCA_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IC_EstablishICP_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    IC_EstablishICP_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_EstablishICP_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_EstablishICP_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_EstablishICP_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_EstablishICP_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IC_MCR_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    IC_MCR_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_MCR_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_MCR_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_MCR_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_MCR_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IC_Notify_AssignId = table.Column<long>(type: "bigint", nullable: true),
                    IC_Notify_ComplateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_Notify_ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_Notify_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_Notify_StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IC_Notify_StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentValidationAssessments_IncidentValidations_IncidentValidationId",
                        column: x => x.IncidentValidationId,
                        principalTable: "IncidentValidations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentValidationAssessments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IncidentValidationRepairs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncidentId = table.Column<long>(type: "bigint", nullable: false),
                    IncidentValidationId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PFO_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PFO_Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventFurtherOutage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventFurtherOutageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SOL_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SOL_Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceOfLeak = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceOfLeakStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VTF_Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VTF_Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VacuumTruckFitting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VacuumTruckFittingStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentValidationRepairs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationAssessments_IncidentId",
                table: "IncidentValidationAssessments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationAssessments_IncidentValidationId",
                table: "IncidentValidationAssessments",
                column: "IncidentValidationId");
        }
    }
}
