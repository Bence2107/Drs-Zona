CREATE TABLE brands (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name text NOT NULL,
    description text NOT NULL,
    principal text NOT NULL,
    headquarters text NOT NULL,
    CONSTRAINT "PK_brands" PRIMARY KEY (id)
);


CREATE TABLE circuits (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name text NOT NULL,
    length double precision NOT NULL,
    type text NOT NULL,
    location text NOT NULL,
    fastest_lap text NOT NULL,
    CONSTRAINT "PK_circuits" PRIMARY KEY (id)
);


CREATE TABLE drivers (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name text NOT NULL,
    nationality text NOT NULL,
    birth_date timestamp with time zone NOT NULL,
    total_races integer NOT NULL,
    wins integer NOT NULL,
    podiums integer NOT NULL,
    championships integer NOT NULL,
    pole_positions integer NOT NULL,
    seasons integer NOT NULL,
    CONSTRAINT "PK_drivers" PRIMARY KEY (id)
);


CREATE TABLE series (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    name text NOT NULL,
    short_name text NOT NULL,
    description text NOT NULL,
    governing_body text NOT NULL,
    first_year integer NOT NULL,
    last_year integer NOT NULL,
    point_system text NOT NULL,
    CONSTRAINT "PK_series" PRIMARY KEY (id)
);


CREATE TABLE users (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    username character varying(50) NOT NULL,
    full_name text NOT NULL,
    email character varying(100) NOT NULL,
    password_hash text NOT NULL,
    role character varying(20) NOT NULL DEFAULT 'user',
    created_at timestamp with time zone NOT NULL,
    has_avatar boolean NOT NULL DEFAULT FALSE,
    last_active timestamp with time zone,
    last_login timestamp with time zone,
    is_logged_in boolean NOT NULL,
    current_session_id character varying(100),
    CONSTRAINT "PK_users" PRIMARY KEY (id)
);


CREATE TABLE constructors (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    brand_id uuid NOT NULL,
    name text NOT NULL,
    nickname text NOT NULL,
    founded_year integer NOT NULL,
    headquarters text NOT NULL,
    team_chief text NOT NULL,
    technical_chief text NOT NULL,
    seasons integer NOT NULL,
    championships integer NOT NULL,
    wins integer NOT NULL,
    podiums integer NOT NULL,
    CONSTRAINT "PK_constructors" PRIMARY KEY (id),
    CONSTRAINT "FK_constructors_brands_brand_id" FOREIGN KEY (brand_id) REFERENCES brands (id)
);


CREATE TABLE constructor_champions (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    series_id uuid NOT NULL,
    name text NOT NULL,
    season text NOT NULL,
    status text NOT NULL,
    CONSTRAINT "PK_constructor_champions" PRIMARY KEY (id),
    CONSTRAINT "FK_constructor_champions_series_series_id" FOREIGN KEY (series_id) REFERENCES series (id) ON DELETE CASCADE
);


CREATE TABLE driver_championships (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    series_id uuid NOT NULL,
    name text NOT NULL,
    season text NOT NULL,
    status text NOT NULL,
    CONSTRAINT "PK_driver_championships" PRIMARY KEY (id),
    CONSTRAINT "FK_driver_championships_series_series_id" FOREIGN KEY (series_id) REFERENCES series (id) ON DELETE CASCADE
);


CREATE TABLE grands_prix (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    circuit_id uuid NOT NULL,
    series_id uuid NOT NULL,
    name text NOT NULL,
    short_name text NOT NULL,
    round_number integer NOT NULL,
    season_year integer NOT NULL,
    start_time timestamp with time zone NOT NULL,
    end_time timestamp with time zone NOT NULL,
    race_distance integer NOT NULL,
    laps_completed integer NOT NULL,
    CONSTRAINT "PK_grands_prix" PRIMARY KEY (id),
    CONSTRAINT "FK_grands_prix_circuits_circuit_id" FOREIGN KEY (circuit_id) REFERENCES circuits (id),
    CONSTRAINT "FK_grands_prix_series_series_id" FOREIGN KEY (series_id) REFERENCES series (id) ON DELETE CASCADE
);


CREATE TABLE articles (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    author_id uuid NOT NULL,
    title text NOT NULL,
    lead text NOT NULL,
    is_summary boolean NOT NULL DEFAULT FALSE,
    slug text NOT NULL,
    tag text NOT NULL,
    first_section text NOT NULL,
    second_section text,
    third_section text,
    fourth_section text,
    last_section text NOT NULL,
    published_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_articles" PRIMARY KEY (id),
    CONSTRAINT "FK_articles_users_author_id" FOREIGN KEY (author_id) REFERENCES users (id) ON DELETE SET NULL
);


CREATE TABLE polls (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    author_id uuid,
    title text NOT NULL,
    tag text NOT NULL,
    description text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    is_active boolean NOT NULL,
    CONSTRAINT "PK_polls" PRIMARY KEY (id),
    CONSTRAINT "FK_polls_users_author_id" FOREIGN KEY (author_id) REFERENCES users (id) ON DELETE SET NULL
);


CREATE TABLE contracts (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    driver_id uuid NOT NULL,
    constructor_id uuid NOT NULL,
    CONSTRAINT "PK_contracts" PRIMARY KEY (id),
    CONSTRAINT "FK_contracts_constructors_constructor_id" FOREIGN KEY (constructor_id) REFERENCES constructors (id),
    CONSTRAINT "FK_contracts_drivers_driver_id" FOREIGN KEY (driver_id) REFERENCES drivers (id)
);


CREATE TABLE constructor_competitions (
    constructors_id uuid NOT NULL,
    constructors_championship_id uuid NOT NULL,
    constructor_name_snapshot text NOT NULL,
    constructor_nickname_snapshot text NOT NULL,
    CONSTRAINT "PK_constructor_competitions" PRIMARY KEY (constructors_id, constructors_championship_id),
    CONSTRAINT "FK_constructor_competitions_constructor_champions_constructors~" FOREIGN KEY (constructors_championship_id) REFERENCES constructor_champions (id),
    CONSTRAINT "FK_constructor_competitions_constructors_constructors_id" FOREIGN KEY (constructors_id) REFERENCES constructors (id)
);


CREATE TABLE driver_participations (
    driver_id uuid NOT NULL,
    drivers_championship_id uuid NOT NULL,
    driver_number integer NOT NULL DEFAULT -1,
    driver_name_snapshot text NOT NULL,
    CONSTRAINT "PK_driver_participations" PRIMARY KEY (driver_id, drivers_championship_id),
    CONSTRAINT "FK_driver_participations_driver_championships_drivers_champion~" FOREIGN KEY (drivers_championship_id) REFERENCES driver_championships (id),
    CONSTRAINT "FK_driver_participations_drivers_driver_id" FOREIGN KEY (driver_id) REFERENCES drivers (id)
);


CREATE TABLE results (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    grand_prix_id uuid NOT NULL,
    driver_id uuid NOT NULL,
    driver_name_snapshot text NOT NULL DEFAULT '',
    constructor_id uuid NOT NULL,
    constructor_name_snapshot text NOT NULL DEFAULT '',
    constructor_nickname_snapshot text NOT NULL DEFAULT '',
    drivers_championship_id uuid NOT NULL,
    constructors_championship_id uuid NOT NULL,
    start_position integer NOT NULL,
    finish_position integer NOT NULL,
    session text NOT NULL,
    car_number integer NOT NULL,
    race_time bigint NOT NULL,
    status text NOT NULL,
    laps_completed integer NOT NULL,
    driver_points double precision NOT NULL,
    constructor_points double precision NOT NULL,
    is_pole_position boolean NOT NULL,
    is_fastest_lap boolean NOT NULL,
    CONSTRAINT "PK_results" PRIMARY KEY (id),
    CONSTRAINT "FK_results_constructor_champions_constructors_championship_id" FOREIGN KEY (constructors_championship_id) REFERENCES constructor_champions (id),
    CONSTRAINT "FK_results_constructors_constructor_id" FOREIGN KEY (constructor_id) REFERENCES constructors (id),
    CONSTRAINT "FK_results_driver_championships_drivers_championship_id" FOREIGN KEY (drivers_championship_id) REFERENCES driver_championships (id),
    CONSTRAINT "FK_results_drivers_driver_id" FOREIGN KEY (driver_id) REFERENCES drivers (id),
    CONSTRAINT "FK_results_grands_prix_grand_prix_id" FOREIGN KEY (grand_prix_id) REFERENCES grands_prix (id)
);


CREATE TABLE comments (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    user_id uuid NOT NULL,
    article_id uuid NOT NULL,
    reply_id uuid,
    content text NOT NULL,
    upvotes integer NOT NULL DEFAULT 0,
    downvotes integer NOT NULL DEFAULT 0,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_comments" PRIMARY KEY (id),
    CONSTRAINT "FK_comments_articles_article_id" FOREIGN KEY (article_id) REFERENCES articles (id) ON DELETE CASCADE,
    CONSTRAINT "FK_comments_comments_reply_id" FOREIGN KEY (reply_id) REFERENCES comments (id) ON DELETE CASCADE,
    CONSTRAINT "FK_comments_users_user_id" FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);


CREATE TABLE poll_options (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    poll_id uuid NOT NULL,
    text text NOT NULL,
    CONSTRAINT "PK_poll_options" PRIMARY KEY (id),
    CONSTRAINT "FK_poll_options_polls_poll_id" FOREIGN KEY (poll_id) REFERENCES polls (id) ON DELETE CASCADE
);


CREATE TABLE qualifying_results (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    result_id uuid NOT NULL,
    q1_time bigint,
    q2_time bigint,
    q3_time bigint,
    CONSTRAINT "PK_qualifying_results" PRIMARY KEY (id),
    CONSTRAINT "FK_qualifying_results_results_result_id" FOREIGN KEY (result_id) REFERENCES results (id) ON DELETE CASCADE
);


CREATE TABLE comment_votes (
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    user_id uuid,
    comment_id uuid NOT NULL,
    is_upvote boolean NOT NULL,
    CONSTRAINT "PK_comment_votes" PRIMARY KEY (id),
    CONSTRAINT "FK_comment_votes_comments_comment_id" FOREIGN KEY (comment_id) REFERENCES comments (id) ON DELETE CASCADE,
    CONSTRAINT "FK_comment_votes_users_user_id" FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE SET NULL
);


CREATE TABLE poll_votes (
    user_id uuid NOT NULL,
    poll_option_id uuid NOT NULL,
    id uuid NOT NULL DEFAULT (gen_random_uuid()),
    CONSTRAINT "PK_poll_votes" PRIMARY KEY (user_id, poll_option_id),
    CONSTRAINT "FK_poll_votes_poll_options_poll_option_id" FOREIGN KEY (poll_option_id) REFERENCES poll_options (id) ON DELETE CASCADE,
    CONSTRAINT "FK_poll_votes_users_user_id" FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE SET NULL
);


CREATE INDEX "IX_articles_author_id" ON articles (author_id);


CREATE INDEX "IX_comment_votes_comment_id" ON comment_votes (comment_id);


CREATE UNIQUE INDEX "IX_comment_votes_user_id_comment_id" ON comment_votes (user_id, comment_id);


CREATE INDEX "IX_comments_article_id" ON comments (article_id);


CREATE INDEX "IX_comments_reply_id" ON comments (reply_id);


CREATE INDEX "IX_comments_user_id" ON comments (user_id);


CREATE INDEX "IX_constructor_champions_series_id" ON constructor_champions (series_id);


CREATE INDEX "IX_constructor_competitions_constructors_championship_id" ON constructor_competitions (constructors_championship_id);


CREATE INDEX "IX_constructors_brand_id" ON constructors (brand_id);


CREATE INDEX "IX_contracts_constructor_id" ON contracts (constructor_id);


CREATE INDEX "IX_contracts_driver_id" ON contracts (driver_id);


CREATE INDEX "IX_driver_championships_series_id" ON driver_championships (series_id);


CREATE INDEX "IX_driver_participations_drivers_championship_id" ON driver_participations (drivers_championship_id);


CREATE INDEX "IX_grands_prix_circuit_id" ON grands_prix (circuit_id);


CREATE INDEX "IX_grands_prix_series_id" ON grands_prix (series_id);


CREATE INDEX "IX_poll_options_poll_id" ON poll_options (poll_id);


CREATE INDEX "IX_poll_votes_poll_option_id" ON poll_votes (poll_option_id);


CREATE INDEX "IX_polls_author_id" ON polls (author_id);


CREATE UNIQUE INDEX "IX_qualifying_results_result_id" ON qualifying_results (result_id);


CREATE INDEX "IX_results_constructor_id" ON results (constructor_id);


CREATE INDEX "IX_results_constructors_championship_id" ON results (constructors_championship_id);


CREATE INDEX "IX_results_driver_id" ON results (driver_id);


CREATE INDEX "IX_results_drivers_championship_id" ON results (drivers_championship_id);


CREATE INDEX "IX_results_grand_prix_id" ON results (grand_prix_id);


