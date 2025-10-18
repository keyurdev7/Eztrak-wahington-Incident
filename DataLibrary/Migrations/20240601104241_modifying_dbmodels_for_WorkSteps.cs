using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class modifying_dbmodels_for_WorkSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskEquipments_Equipments_EquipmentId",
                table: "TaskEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskMaterials_Inventories_MaterialId",
                table: "TaskMaterials");

            migrationBuilder.DropTable(
                name: "TaskCraftSkills");

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

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "TaskTypes",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Hours",
                table: "TaskTypes",
                newName: "BudgetHours");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TaskTypes",
                newName: "Title");

            migrationBuilder.AddColumn<double>(
                name: "BudgetCost",
                table: "TaskTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                table: "TaskMaterials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TaskMaterials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                table: "TaskEquipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TaskEquipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TaskLabors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    TaskTypeId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLabors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLabors_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskWorkSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TaskTypeId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskWorkSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskWorkSteps_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabors_TaskTypeId",
                table: "TaskLabors",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskWorkSteps_TaskTypeId",
                table: "TaskWorkSteps",
                column: "TaskTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskLabors");

            migrationBuilder.DropTable(
                name: "TaskWorkSteps");

            migrationBuilder.DropColumn(
                name: "BudgetCost",
                table: "TaskTypes");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "TaskMaterials");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TaskMaterials");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "TaskEquipments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TaskEquipments");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TaskTypes",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "TaskTypes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "BudgetHours",
                table: "TaskTypes",
                newName: "Hours");

            migrationBuilder.AddColumn<long>(
                name: "MaterialId",
                table: "TaskMaterials",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EquipmentId",
                table: "TaskEquipments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TaskCraftSkills",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CraftSkillId = table.Column<long>(type: "bigint", nullable: false),
                    TaskTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCraftSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCraftSkills_CraftSkills_CraftSkillId",
                        column: x => x.CraftSkillId,
                        principalTable: "CraftSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCraftSkills_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskMaterials_MaterialId",
                table: "TaskMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEquipments_EquipmentId",
                table: "TaskEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCraftSkills_CraftSkillId",
                table: "TaskCraftSkills",
                column: "CraftSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCraftSkills_TaskTypeId",
                table: "TaskCraftSkills",
                column: "TaskTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEquipments_Equipments_EquipmentId",
                table: "TaskEquipments",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskMaterials_Inventories_MaterialId",
                table: "TaskMaterials",
                column: "MaterialId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
