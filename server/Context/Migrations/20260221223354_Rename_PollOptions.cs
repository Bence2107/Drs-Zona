using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Rename_PollOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_poll_votes_pollOptions_poll_option_id",
                table: "poll_votes");

            migrationBuilder.DropForeignKey(
                name: "FK_pollOptions_polls_poll_id",
                table: "pollOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pollOptions",
                table: "pollOptions");

            migrationBuilder.RenameTable(
                name: "pollOptions",
                newName: "poll_options");

            migrationBuilder.RenameIndex(
                name: "IX_pollOptions_poll_id",
                table: "poll_options",
                newName: "IX_poll_options_poll_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_poll_options",
                table: "poll_options",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_poll_options_polls_poll_id",
                table: "poll_options",
                column: "poll_id",
                principalTable: "polls",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_poll_votes_poll_options_poll_option_id",
                table: "poll_votes",
                column: "poll_option_id",
                principalTable: "poll_options",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_poll_options_polls_poll_id",
                table: "poll_options");

            migrationBuilder.DropForeignKey(
                name: "FK_poll_votes_poll_options_poll_option_id",
                table: "poll_votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_poll_options",
                table: "poll_options");

            migrationBuilder.RenameTable(
                name: "poll_options",
                newName: "pollOptions");

            migrationBuilder.RenameIndex(
                name: "IX_poll_options_poll_id",
                table: "pollOptions",
                newName: "IX_pollOptions_poll_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pollOptions",
                table: "pollOptions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_poll_votes_pollOptions_poll_option_id",
                table: "poll_votes",
                column: "poll_option_id",
                principalTable: "pollOptions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pollOptions_polls_poll_id",
                table: "pollOptions",
                column: "poll_id",
                principalTable: "polls",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
