using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdAndIncidentRoleIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "IncidentUsers",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IncidentRoleId",
                table: "IncidentUsers",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentUsers_CompanyId",
                table: "IncidentUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentUsers_IncidentRoleId",
                table: "IncidentUsers",
                column: "IncidentRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentUsers_Company_CompanyId",
                table: "IncidentUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentUsers_IncidentRoles_IncidentRoleId",
                table: "IncidentUsers",
                column: "IncidentRoleId",
                principalTable: "IncidentRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentUsers_Company_CompanyId",
                table: "IncidentUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentUsers_IncidentRoles_IncidentRoleId",
                table: "IncidentUsers");

            migrationBuilder.DropIndex(
                name: "IX_IncidentUsers_CompanyId",
                table: "IncidentUsers");

            migrationBuilder.DropIndex(
                name: "IX_IncidentUsers_IncidentRoleId",
                table: "IncidentUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "IncidentUsers");

            migrationBuilder.DropColumn(
                name: "IncidentRoleId",
                table: "IncidentUsers");
        }
    }
}
