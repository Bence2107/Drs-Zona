using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class User_lastLogin_lastActive_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "users",
                newName: "is_logged_in");

            migrationBuilder.AddColumn<string>(
                name: "current_session_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_active",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_session_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_active",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "is_logged_in",
                table: "users",
                newName: "is_active");
        }
    }
}
