using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class CorrectAspNetUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
