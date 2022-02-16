using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class CorrectMainImageColumnNameInBlogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Pages",
                newName: "MainImageName");

            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Blogs",
                newName: "MainImageName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainImageName",
                table: "Pages",
                newName: "MainImageUrl");

            migrationBuilder.RenameColumn(
                name: "MainImageName",
                table: "Blogs",
                newName: "MainImageUrl");
        }
    }
}
