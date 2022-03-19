using Microsoft.EntityFrameworkCore.Migrations;

namespace KiddieParadies.Data.Migrations
{
    public partial class FixTripsRelationWithEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_AspNetUsers_DriverId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_AspNetUsers_EscortId",
                table: "Trips");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_YearEmployees_DriverId",
                table: "Trips",
                column: "DriverId",
                principalTable: "YearEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_YearEmployees_EscortId",
                table: "Trips",
                column: "EscortId",
                principalTable: "YearEmployees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_YearEmployees_DriverId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_YearEmployees_EscortId",
                table: "Trips");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_AspNetUsers_DriverId",
                table: "Trips",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_AspNetUsers_EscortId",
                table: "Trips",
                column: "EscortId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
