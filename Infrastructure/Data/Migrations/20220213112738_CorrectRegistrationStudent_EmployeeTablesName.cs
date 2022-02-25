using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace KiddieParadies.Data.Migrations
{
    public partial class CorrectRegistrationStudent_EmployeeTablesName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRegisterationsInfos");

            migrationBuilder.DropTable(
                name: "StuentRegisterationsInfos");

            migrationBuilder.CreateTable(
                name: "EmployeeRegistrationsInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Teacher = table.Column<int>(type: "int", nullable: false),
                    Driver = table.Column<int>(type: "int", nullable: false),
                    Escort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRegistrationsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeRegistrationsInfos_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentRegistrationsInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LevelOne = table.Column<int>(type: "int", nullable: false),
                    LevelTwo = table.Column<int>(type: "int", nullable: false),
                    LevelThree = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRegistrationsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRegistrationsInfos_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRegistrationsInfos_YearId",
                table: "EmployeeRegistrationsInfos",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRegistrationsInfos_YearId",
                table: "StudentRegistrationsInfos",
                column: "YearId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRegistrationsInfos");

            migrationBuilder.DropTable(
                name: "StudentRegistrationsInfos");

            migrationBuilder.CreateTable(
                name: "EmployeeRegisterationsInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Driver = table.Column<int>(type: "int", nullable: false),
                    Escort = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Teacher = table.Column<int>(type: "int", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRegisterationsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeRegisterationsInfos_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StuentRegisterationsInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LevelOne = table.Column<int>(type: "int", nullable: false),
                    LevelThree = table.Column<int>(type: "int", nullable: false),
                    LevelTwo = table.Column<int>(type: "int", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StuentRegisterationsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StuentRegisterationsInfos_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRegisterationsInfos_YearId",
                table: "EmployeeRegisterationsInfos",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_StuentRegisterationsInfos_YearId",
                table: "StuentRegisterationsInfos",
                column: "YearId");
        }
    }
}
