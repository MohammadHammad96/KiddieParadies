using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class RenameFatherLastNameColumnInParentsTableAndAddUniqueStudentNameForParentInStudentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FatherLaseName",
                table: "Parents",
                newName: "FatherLastName");

            migrationBuilder.CreateIndex(
                name: "IX_Students_FirstName_ParentId",
                table: "Students",
                columns: new[] { "FirstName", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_FirstName_ParentId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "FatherLastName",
                table: "Parents",
                newName: "FatherLaseName");
        }
    }
}
