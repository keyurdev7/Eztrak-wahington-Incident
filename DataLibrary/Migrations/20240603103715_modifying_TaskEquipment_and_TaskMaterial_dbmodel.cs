using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class modifying_TaskEquipment_and_TaskMaterial_dbmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "TaskMaterials");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TaskEquipments");

            migrationBuilder.AddColumn<long>(
                name: "MaterialId",
                table: "TaskMaterials",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EquipmentId",
                table: "TaskEquipments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskMaterials_MaterialId",
                table: "TaskMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEquipments_EquipmentId",
                table: "TaskEquipments",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEquipments_Equipments_EquipmentId",
                table: "TaskEquipments",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskMaterials_Inventories_MaterialId",
                table: "TaskMaterials",
                column: "MaterialId",
                principalTable: "Inventories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskEquipments_Equipments_EquipmentId",
                table: "TaskEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskMaterials_Inventories_MaterialId",
                table: "TaskMaterials");

            migrationBuilder.DropIndex(
                name: "IX_TaskMaterials_MaterialId",
                table: "TaskMaterials");

            migrationBuilder.DropIndex(
                name: "IX_TaskEquipments_EquipmentId",
                table: "TaskEquipments");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "TaskMaterials");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "TaskEquipments");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TaskMaterials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TaskEquipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
