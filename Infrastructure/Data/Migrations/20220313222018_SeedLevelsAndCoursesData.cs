using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace KiddieParadies.Data.Migrations
{
    public partial class SeedLevelsAndCoursesData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("Levels", new[] { "Order", "Name" },
                new Object[,]
                {
                    { 3, "أول" },
                    { 4, "ثاني" },
                    { 5, "ثالث" }
                });

            migrationBuilder.InsertData("Courses", new[] { "Name" },
                new Object[,]
                {
                    { "رياضيات" },
                    { "لغة عربية" },
                    { "لغة إنجليزية" },
                    { "رياضة" },
                    { "موسيقى" },
                    { "رسم" },
                    { "حاسوب" },
                    { "نشاط ترفيهي" },
                    { "ألعاب" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Levels");

            migrationBuilder.Sql("DELETE FROM Courses");
        }
    }
}
