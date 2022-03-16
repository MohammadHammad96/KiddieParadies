using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class AddDiscriminatorOnApsNetUserRolesAndTriggerForIt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("CREATE TRIGGER UpdateDiscriminator ON AspNetUserRoles AFTER INSERT AS BEGIN UPDATE AspNetUserRoles SET Discriminator='ApplicationUserRole' END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");

            migrationBuilder.Sql("DROP TRIGGER UpdateDiscriminator");
        }
    }
}
