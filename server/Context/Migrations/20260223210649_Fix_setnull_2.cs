using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Fix_setnull_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_comment_votes",
                table: "comment_votes");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "poll_votes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "comment_votes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "comment_votes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comment_votes",
                table: "comment_votes",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_votes_user_id_comment_id",
                table: "comment_votes",
                columns: new[] { "user_id", "comment_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_comment_votes",
                table: "comment_votes");

            migrationBuilder.DropIndex(
                name: "IX_comment_votes_user_id_comment_id",
                table: "comment_votes");

            migrationBuilder.DropColumn(
                name: "id",
                table: "poll_votes");

            migrationBuilder.DropColumn(
                name: "id",
                table: "comment_votes");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "comment_votes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_comment_votes",
                table: "comment_votes",
                columns: new[] { "user_id", "comment_id" });
        }
    }
}
