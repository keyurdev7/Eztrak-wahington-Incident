using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddrefIncidentValidationsNewTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IncidentValidationId",
                table: "IncidentValidationPolicies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IncidentValidationId",
                table: "IncidentValidationCommunicationHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationPolicies_IncidentValidationId",
                table: "IncidentValidationPolicies",
                column: "IncidentValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentValidationCommunicationHistories_IncidentValidationId",
                table: "IncidentValidationCommunicationHistories",
                column: "IncidentValidationId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentValidationCommunicationHistories_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationCommunicationHistories",
                column: "IncidentValidationId",
                principalTable: "IncidentValidations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentValidationPolicies_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationPolicies",
                column: "IncidentValidationId",
                principalTable: "IncidentValidations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentValidationCommunicationHistories_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationCommunicationHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentValidationPolicies_IncidentValidations_IncidentValidationId",
                table: "IncidentValidationPolicies");

            migrationBuilder.DropIndex(
                name: "IX_IncidentValidationPolicies_IncidentValidationId",
                table: "IncidentValidationPolicies");

            migrationBuilder.DropIndex(
                name: "IX_IncidentValidationCommunicationHistories_IncidentValidationId",
                table: "IncidentValidationCommunicationHistories");

            migrationBuilder.DropColumn(
                name: "IncidentValidationId",
                table: "IncidentValidationPolicies");

            migrationBuilder.DropColumn(
                name: "IncidentValidationId",
                table: "IncidentValidationCommunicationHistories");
        }
    }
}
