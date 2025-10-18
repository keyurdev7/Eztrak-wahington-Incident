using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class addingrelationshipbetweenStreetServiceRequestandWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_StreetServiceRequestId",
                table: "WorkOrder",
                column: "StreetServiceRequestId",
                unique: true,
                filter: "[StreetServiceRequestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_StreetServiceRequests_StreetServiceRequestId",
                table: "WorkOrder",
                column: "StreetServiceRequestId",
                principalTable: "StreetServiceRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_StreetServiceRequests_StreetServiceRequestId",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_StreetServiceRequestId",
                table: "WorkOrder");

        }
    }
}
