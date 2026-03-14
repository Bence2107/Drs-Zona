using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class AddQualifyingResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "qualifying_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    result_id = table.Column<Guid>(type: "uuid", nullable: false),
                    q1_time = table.Column<long>(type: "bigint", nullable: true),
                    q2_time = table.Column<long>(type: "bigint", nullable: true),
                    q3_time = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qualifying_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_qualifying_results_results_result_id",
                        column: x => x.result_id,
                        principalTable: "results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_qualifying_results_result_id",
                table: "qualifying_results",
                column: "result_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qualifying_results");
        }
    }
}
