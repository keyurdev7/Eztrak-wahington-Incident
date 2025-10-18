using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class IncidentValidationAssessmentAddnewCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_ICT_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_ICT_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_ICT_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_ICT_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_MLP_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_MLP_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_MLP_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_MLP_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_RSM_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_RSM_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EGEC_RSM_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EGEC_RSM_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FER_LC_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FER_LC_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FER_LC_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FER_LC_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FER_PCA_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FER_PCA_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FER_PCA_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FER_PCA_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_EstablishICP_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_EstablishICP_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_EstablishICP_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_EstablishICP_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_MCR_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_MCR_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_MCR_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_MCR_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_Notify_ComplateTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_Notify_ImageUrls",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IC_Notify_Notes",
                table: "IncidentValidationAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IC_Notify_StartTime",
                table: "IncidentValidationAssessments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EGEC_ICT_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_ICT_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_ICT_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_ICT_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_MLP_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_MLP_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_MLP_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_MLP_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_RSM_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_RSM_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_RSM_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "EGEC_RSM_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_LC_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_LC_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_LC_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_LC_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_PCA_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_PCA_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_PCA_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "FER_PCA_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_EstablishICP_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_EstablishICP_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_EstablishICP_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_EstablishICP_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_MCR_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_MCR_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_MCR_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_MCR_StartTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_Notify_ComplateTime",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_Notify_ImageUrls",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_Notify_Notes",
                table: "IncidentValidationAssessments");

            migrationBuilder.DropColumn(
                name: "IC_Notify_StartTime",
                table: "IncidentValidationAssessments");
        }
    }
}
