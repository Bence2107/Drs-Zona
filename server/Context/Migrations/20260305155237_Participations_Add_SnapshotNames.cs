using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Participations_Add_SnapshotNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "driver_name_snapshot",
                table: "driver_participations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "constructor_name_snapshot",
                table: "constructor_competitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "constructor_nickname_snapshot",
                table: "constructor_competitions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "driver_name_snapshot",
                table: "driver_participations");

            migrationBuilder.DropColumn(
                name: "constructor_name_snapshot",
                table: "constructor_competitions");

            migrationBuilder.DropColumn(
                name: "constructor_nickname_snapshot",
                table: "constructor_competitions");
        }
    }
}
