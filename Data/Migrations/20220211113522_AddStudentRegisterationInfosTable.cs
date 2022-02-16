using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class AddStudentRegisterationInfosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StuentRegisterationsInfos",
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
                    table.PrimaryKey("PK_StuentRegisterationsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StuentRegisterationsInfos_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StuentRegisterationsInfos_YearId",
                table: "StuentRegisterationsInfos",
                column: "YearId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StuentRegisterationsInfos");
        }
    }
}
