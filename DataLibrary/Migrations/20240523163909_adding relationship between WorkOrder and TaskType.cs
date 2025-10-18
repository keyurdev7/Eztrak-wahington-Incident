using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class addingrelationshipbetweenWorkOrderandTaskType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TaskTypeId",
                table: "WorkOrder",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_TaskTypeId",
                table: "WorkOrder",
                column: "TaskTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_TaskTypes_TaskTypeId",
                table: "WorkOrder",
                column: "TaskTypeId",
                principalTable: "TaskTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_TaskTypes_TaskTypeId",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_TaskTypeId",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "TaskTypeId",
                table: "WorkOrder");
        }
    }
}
