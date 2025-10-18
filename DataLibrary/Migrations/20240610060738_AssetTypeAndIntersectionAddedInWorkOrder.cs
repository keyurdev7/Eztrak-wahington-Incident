using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AssetTypeAndIntersectionAddedInWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssetTypeId",
                table: "WorkOrder",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intersection",
                table: "WorkOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_AssetTypeId",
                table: "WorkOrder",
                column: "AssetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_AssetTypes_AssetTypeId",
                table: "WorkOrder",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_AssetTypes_AssetTypeId",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_AssetTypeId",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "Intersection",
                table: "WorkOrder");
        }
    }
}
