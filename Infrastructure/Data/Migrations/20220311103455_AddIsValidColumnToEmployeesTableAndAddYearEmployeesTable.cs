using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class AddIsValidColumnToEmployeesTableAndAddYearEmployeesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "YearEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ToDate = table.Column<DateTime>(type: "Date", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YearEmployees_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YearEmployees_EmployeeId",
                table: "YearEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_YearEmployees_YearId_EmployeeId",
                table: "YearEmployees",
                columns: new[] { "YearId", "EmployeeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YearEmployees");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Employees");
        }
    }
}
