using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class FixTripsRelationWithStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TripId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_TripId",
                table: "Students",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Trips_TripId",
                table: "Students",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Trips_TripId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TripId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "Students");
        }
    }
}
