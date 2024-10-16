using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SpotNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpotNumber",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotNumber",
                table: "CustomerOrders");
        }
    }
}
