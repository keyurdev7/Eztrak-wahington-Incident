using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class adding_TaskTypeStep_dbmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskTypeSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Men = table.Column<double>(type: "float", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Material = table.Column<double>(type: "float", nullable: false),
                    Equipment = table.Column<double>(type: "float", nullable: false),
                    CraftId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_TaskTypeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTypeSteps_CraftSkills_CraftId",
                        column: x => x.CraftId,
                        principalTable: "CraftSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTypeSteps_TaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypeSteps_CraftId",
                table: "TaskTypeSteps",
                column: "CraftId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypeSteps_TaskTypeId",
                table: "TaskTypeSteps",
                column: "TaskTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTypeSteps");
        }
    }
}
