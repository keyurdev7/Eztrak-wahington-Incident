using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class LaborEquipmentandAssetcostcolumntoWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AssetCost",
                table: "WorkOrder",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EquipmentCost",
                table: "WorkOrder",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LabourCost",
                table: "WorkOrder",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetCost",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "EquipmentCost",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "LabourCost",
                table: "WorkOrder");
        }
    }
}
