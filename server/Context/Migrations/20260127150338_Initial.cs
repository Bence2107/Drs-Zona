using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    principal = table.Column<string>(type: "text", nullable: false),
                    headquarters = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "circuits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    length = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    fastest_lap = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_circuits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "drivers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    nationality = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    driver_number = table.Column<int>(type: "integer", nullable: false),
                    total_races = table.Column<int>(type: "integer", nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    podiums = table.Column<int>(type: "integer", nullable: false),
                    championships = table.Column<int>(type: "integer", nullable: false),
                    pole_positions = table.Column<int>(type: "integer", nullable: false),
                    seasons = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drivers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    governing_body = table.Column<string>(type: "text", nullable: false),
                    first_year = table.Column<int>(type: "integer", nullable: false),
                    last_year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_series", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "user"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "constructors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    founded_year = table.Column<int>(type: "integer", nullable: false),
                    headquarters = table.Column<string>(type: "text", nullable: false),
                    team_chief = table.Column<string>(type: "text", nullable: false),
                    technical_chief = table.Column<string>(type: "text", nullable: false),
                    seasons = table.Column<int>(type: "integer", nullable: false),
                    championships = table.Column<int>(type: "integer", nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    podiums = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constructors", x => x.id);
                    table.ForeignKey(
                        name: "FK_constructors_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "constructor_champions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    season = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constructor_champions", x => x.id);
                    table.ForeignKey(
                        name: "FK_constructor_champions_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_championships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    season = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_championships", x => x.id);
                    table.ForeignKey(
                        name: "FK_driver_championships_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grands_prix",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    circuit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    round_number = table.Column<int>(type: "integer", nullable: false),
                    season_year = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    race_distance = table.Column<int>(type: "integer", nullable: false),
                    laps_completed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grands_prix", x => x.id);
                    table.ForeignKey(
                        name: "FK_grands_prix_circuits_circuit_id",
                        column: x => x.circuit_id,
                        principalTable: "circuits",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_grands_prix_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "polls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_polls", x => x.id);
                    table.ForeignKey(
                        name: "FK_polls_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    constructor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.id);
                    table.ForeignKey(
                        name: "FK_contracts_constructors_constructor_id",
                        column: x => x.constructor_id,
                        principalTable: "constructors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contracts_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "constructor_competitions",
                columns: table => new
                {
                    constructors_id = table.Column<Guid>(type: "uuid", nullable: false),
                    constructors_championship_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constructor_competitions", x => new { x.constructors_id, x.constructors_championship_id });
                    table.ForeignKey(
                        name: "FK_constructor_competitions_constructor_champions_constructors~",
                        column: x => x.constructors_championship_id,
                        principalTable: "constructor_champions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_constructor_competitions_constructors_constructors_id",
                        column: x => x.constructors_id,
                        principalTable: "constructors",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "driver_participations",
                columns: table => new
                {
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drivers_championship_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_participations", x => new { x.driver_id, x.drivers_championship_id });
                    table.ForeignKey(
                        name: "FK_driver_participations_driver_championships_drivers_champion~",
                        column: x => x.drivers_championship_id,
                        principalTable: "driver_championships",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_driver_participations_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grand_prix_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    lead = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    first_section = table.Column<string>(type: "text", nullable: false),
                    second_section = table.Column<string>(type: "text", nullable: false),
                    third_section = table.Column<string>(type: "text", nullable: false),
                    fourth_section = table.Column<string>(type: "text", nullable: false),
                    last_section = table.Column<string>(type: "text", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.id);
                    table.ForeignKey(
                        name: "FK_articles_grands_prix_grand_prix_id",
                        column: x => x.grand_prix_id,
                        principalTable: "grands_prix",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_articles_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    grand_prix_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    constructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drivers_championship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    constructors_championship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_position = table.Column<int>(type: "integer", nullable: false),
                    finish_position = table.Column<int>(type: "integer", nullable: false),
                    session = table.Column<string>(type: "text", nullable: false),
                    race_time = table.Column<long>(type: "bigint", nullable: false),
                    driver_points = table.Column<int>(type: "integer", nullable: false),
                    constructor_points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_results_constructor_champions_constructors_championship_id",
                        column: x => x.constructors_championship_id,
                        principalTable: "constructor_champions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_results_constructors_constructor_id",
                        column: x => x.constructor_id,
                        principalTable: "constructors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_results_driver_championships_drivers_championship_id",
                        column: x => x.drivers_championship_id,
                        principalTable: "driver_championships",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_results_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_results_grands_prix_grand_prix_id",
                        column: x => x.grand_prix_id,
                        principalTable: "grands_prix",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pollOptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    poll_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pollOptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_pollOptions_polls_poll_id",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reply_id = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    upvotes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    downvotes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comments_comments_reply_id",
                        column: x => x.reply_id,
                        principalTable: "comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "votes",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    poll_option_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votes", x => new { x.user_id, x.poll_option_id });
                    table.ForeignKey(
                        name: "FK_votes_pollOptions_poll_option_id",
                        column: x => x.poll_option_id,
                        principalTable: "pollOptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_votes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_articles_author_id",
                table: "articles",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_articles_grand_prix_id",
                table: "articles",
                column: "grand_prix_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_article_id",
                table: "comments",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_reply_id",
                table: "comments",
                column: "reply_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_constructor_champions_series_id",
                table: "constructor_champions",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "IX_constructor_competitions_constructors_championship_id",
                table: "constructor_competitions",
                column: "constructors_championship_id");

            migrationBuilder.CreateIndex(
                name: "IX_constructors_brand_id",
                table: "constructors",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_constructor_id",
                table: "contracts",
                column: "constructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_driver_id",
                table: "contracts",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_championships_series_id",
                table: "driver_championships",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_participations_drivers_championship_id",
                table: "driver_participations",
                column: "drivers_championship_id");

            migrationBuilder.CreateIndex(
                name: "IX_grands_prix_circuit_id",
                table: "grands_prix",
                column: "circuit_id");

            migrationBuilder.CreateIndex(
                name: "IX_grands_prix_series_id",
                table: "grands_prix",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "IX_pollOptions_poll_id",
                table: "pollOptions",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "IX_polls_author_id",
                table: "polls",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_constructor_id",
                table: "results",
                column: "constructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_constructors_championship_id",
                table: "results",
                column: "constructors_championship_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_driver_id",
                table: "results",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_drivers_championship_id",
                table: "results",
                column: "drivers_championship_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_grand_prix_id",
                table: "results",
                column: "grand_prix_id");

            migrationBuilder.CreateIndex(
                name: "IX_votes_poll_option_id",
                table: "votes",
                column: "poll_option_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "constructor_competitions");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "driver_participations");

            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "votes");

            migrationBuilder.DropTable(
                name: "articles");

            migrationBuilder.DropTable(
                name: "constructor_champions");

            migrationBuilder.DropTable(
                name: "constructors");

            migrationBuilder.DropTable(
                name: "driver_championships");

            migrationBuilder.DropTable(
                name: "drivers");

            migrationBuilder.DropTable(
                name: "pollOptions");

            migrationBuilder.DropTable(
                name: "grands_prix");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "polls");

            migrationBuilder.DropTable(
                name: "circuits");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
