using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class adding_StreetServiceRequest_dbmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StreetServiceRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CallMe = table.Column<bool>(type: "bit", nullable: false),
                    EmailMe = table.Column<bool>(type: "bit", nullable: false),
                    NoNeedToContact = table.Column<bool>(type: "bit", nullable: false),
                    SideWalkDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PotholesDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrainageDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetSweepingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkwayTreeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationOfProblem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionOfProblem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetServiceRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetServiceRequests");
        }
    }
}
