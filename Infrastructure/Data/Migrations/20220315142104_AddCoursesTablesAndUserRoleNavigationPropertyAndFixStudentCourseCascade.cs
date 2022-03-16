using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace KiddieParadies.Data.Migrations
{
    public partial class AddCoursesTablesAndUserRoleNavigationPropertyAndFixStudentCourseCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CourseClassRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassRoom = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseClassRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseClassRooms_LevelCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "LevelCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseClassRooms_YearEmployees_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "YearEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_CourseClassRooms_CourseId",
                        column: x => x.CourseId,
                        principalTable: "CourseClassRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourses_CourseClassRooms_CourseId",
                        column: x => x.CourseId,
                        principalTable: "CourseClassRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourses_YearStudents_StudentId",
                        column: x => x.StudentId,
                        principalTable: "YearStudents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseClassRooms_ClassRoom_Day_Order_CourseId",
                table: "CourseClassRooms",
                columns: new[] { "ClassRoom", "Day", "Order", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseClassRooms_CourseId",
                table: "CourseClassRooms",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseClassRooms_TeacherId",
                table: "CourseClassRooms",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_CourseId",
                table: "StudentCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudentId_CourseId",
                table: "StudentCourses",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_YearStudentId",
                table: "StudentCourses",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "StudentCourses");

            migrationBuilder.DropTable(
                name: "CourseClassRooms");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");
        }
    }
}
