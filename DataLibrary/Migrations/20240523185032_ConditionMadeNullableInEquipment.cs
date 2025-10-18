using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ConditionMadeNullableInEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTransactions_Conditions_ConditionId",
                table: "EquipmentTransactions");

            migrationBuilder.AlterColumn<long>(
                name: "ConditionId",
                table: "EquipmentTransactions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTransactions_Conditions_ConditionId",
                table: "EquipmentTransactions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTransactions_Conditions_ConditionId",
                table: "EquipmentTransactions");

            migrationBuilder.AlterColumn<long>(
                name: "ConditionId",
                table: "EquipmentTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTransactions_Conditions_ConditionId",
                table: "EquipmentTransactions",
                column: "ConditionId",
                principalTable: "Conditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
