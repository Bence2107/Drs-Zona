using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Update_DeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles");

            migrationBuilder.DropForeignKey(
                name: "FK_comment_votes_users_user_id",
                table: "comment_votes");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_articles_article_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_reply_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_contracts_constructors_constructor_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_contracts_drivers_driver_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_poll_votes_users_user_id",
                table: "poll_votes");

            migrationBuilder.DropForeignKey(
                name: "FK_polls_users_author_id",
                table: "polls");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_comment_votes_users_user_id",
                table: "comment_votes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_articles_article_id",
                table: "comments",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_reply_id",
                table: "comments",
                column: "reply_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contracts_constructors_constructor_id",
                table: "contracts",
                column: "constructor_id",
                principalTable: "constructors",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_contracts_drivers_driver_id",
                table: "contracts",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_poll_votes_users_user_id",
                table: "poll_votes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_polls_users_author_id",
                table: "polls",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles");

            migrationBuilder.DropForeignKey(
                name: "FK_comment_votes_users_user_id",
                table: "comment_votes");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_articles_article_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_reply_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_contracts_constructors_constructor_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_contracts_drivers_driver_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_poll_votes_users_user_id",
                table: "poll_votes");

            migrationBuilder.DropForeignKey(
                name: "FK_polls_users_author_id",
                table: "polls");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_users_author_id",
                table: "articles",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_votes_users_user_id",
                table: "comment_votes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_articles_article_id",
                table: "comments",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_reply_id",
                table: "comments",
                column: "reply_id",
                principalTable: "comments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_contracts_constructors_constructor_id",
                table: "contracts",
                column: "constructor_id",
                principalTable: "constructors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contracts_drivers_driver_id",
                table: "contracts",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_poll_votes_users_user_id",
                table: "poll_votes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_polls_users_author_id",
                table: "polls",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
