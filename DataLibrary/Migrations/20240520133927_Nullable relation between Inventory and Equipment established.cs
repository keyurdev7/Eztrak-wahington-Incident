using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class NullablerelationbetweenInventoryandEquipmentestablished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Inventories_InventoryId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<long>(
                name: "InventoryId",
                table: "OrderItems",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "EquipmentId",
                table: "OrderItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_EquipmentId",
                table: "OrderItems",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Equipments_EquipmentId",
                table: "OrderItems",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Inventories_InventoryId",
                table: "OrderItems",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Equipments_EquipmentId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Inventories_InventoryId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_EquipmentId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<long>(
                name: "InventoryId",
                table: "OrderItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Inventories_InventoryId",
                table: "OrderItems",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
