using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBookingApp.Migrations
{
    public partial class LastEventBookingApp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfEvent",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EventTime",
                table: "CreateEvent",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeOfEvent",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "CreateEvent");
        }
    }
}
