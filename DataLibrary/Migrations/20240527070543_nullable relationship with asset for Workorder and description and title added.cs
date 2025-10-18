using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class nullablerelationshipwithassetforWorkorderanddescriptionandtitleadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Assets_AssetId",
                table: "WorkOrder");

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "WorkOrder",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "WorkOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Assets_AssetId",
                table: "WorkOrder",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Assets_AssetId",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "WorkOrder");

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "WorkOrder",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Assets_AssetId",
                table: "WorkOrder",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
