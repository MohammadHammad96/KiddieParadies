using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("alter table trips alter column Time datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
