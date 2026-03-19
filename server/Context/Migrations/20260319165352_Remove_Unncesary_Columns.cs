using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Unncesary_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_articles_grands_prix_grand_prix_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "IX_articles_grand_prix_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "race_duration_minutes",
                table: "grands_prix");

            migrationBuilder.DropColumn(
                name: "grand_prix_id",
                table: "articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "race_duration_minutes",
                table: "grands_prix",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "grand_prix_id",
                table: "articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_articles_grand_prix_id",
                table: "articles",
                column: "grand_prix_id");

            migrationBuilder.AddForeignKey(
                name: "FK_articles_grands_prix_grand_prix_id",
                table: "articles",
                column: "grand_prix_id",
                principalTable: "grands_prix",
                principalColumn: "id");
        }
    }
}
