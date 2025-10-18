using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class adding_SystemGeneratedId_in_Assets_WorkOrder_Inventory_Equipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WorkOrder",
                newName: "SystemGeneratedId");

            migrationBuilder.AddColumn<string>(
                name: "SystemGeneratedId",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemGeneratedId",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemGeneratedId",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemGeneratedId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "SystemGeneratedId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "SystemGeneratedId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "SystemGeneratedId",
                table: "WorkOrder",
                newName: "Name");
        }
    }
}
