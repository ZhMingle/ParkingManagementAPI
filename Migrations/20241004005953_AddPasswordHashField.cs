﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "SystemUsers");
        }
    }
}
