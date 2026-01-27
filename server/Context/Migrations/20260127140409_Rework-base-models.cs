using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Reworkbasemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "articles",
                newName: "third_section");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "polls",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "series_id",
                table: "grands_prix",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "championships",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "podiums",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pole_positions",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "seasons",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_races",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "wins",
                table: "drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "championships",
                table: "constructors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "podiums",
                table: "constructors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "seasons",
                table: "constructors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "wins",
                table: "constructors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "reply_id",
                table: "comments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "first_section",
                table: "articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fourth_section",
                table: "articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_section",
                table: "articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "second_section",
                table: "articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_grands_prix_series_id",
                table: "grands_prix",
                column: "series_id");

            migrationBuilder.AddForeignKey(
                name: "FK_grands_prix_series_series_id",
                table: "grands_prix",
                column: "series_id",
                principalTable: "series",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_grands_prix_series_series_id",
                table: "grands_prix");

            migrationBuilder.DropIndex(
                name: "IX_grands_prix_series_id",
                table: "grands_prix");

            migrationBuilder.DropColumn(
                name: "title",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "grands_prix");

            migrationBuilder.DropColumn(
                name: "championships",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "podiums",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "pole_positions",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "seasons",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "total_races",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "wins",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "championships",
                table: "constructors");

            migrationBuilder.DropColumn(
                name: "podiums",
                table: "constructors");

            migrationBuilder.DropColumn(
                name: "seasons",
                table: "constructors");

            migrationBuilder.DropColumn(
                name: "wins",
                table: "constructors");

            migrationBuilder.DropColumn(
                name: "first_section",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "fourth_section",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "last_section",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "second_section",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "articles");

            migrationBuilder.RenameColumn(
                name: "third_section",
                table: "articles",
                newName: "content");

            migrationBuilder.AlterColumn<int>(
                name: "reply_id",
                table: "comments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
