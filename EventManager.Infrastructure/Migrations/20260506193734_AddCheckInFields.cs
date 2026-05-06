using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "check_in_token",
                table: "registrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "checked_in",
                table: "registrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "checked_in_at",
                table: "registrations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingCapacity",
                table: "events",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_registrations_check_in_token",
                table: "registrations",
                column: "check_in_token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_registrations_check_in_token",
                table: "registrations");

            migrationBuilder.DropColumn(
                name: "check_in_token",
                table: "registrations");

            migrationBuilder.DropColumn(
                name: "checked_in",
                table: "registrations");

            migrationBuilder.DropColumn(
                name: "checked_in_at",
                table: "registrations");

            migrationBuilder.DropColumn(
                name: "ParkingCapacity",
                table: "events");
        }
    }
}
