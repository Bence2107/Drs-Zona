# Data Model

## Perzisztencia
- **DB:** PostgreSQL
- **ORM:** Entity Framework Core
- **Séma fájl:** [schema.sql](./schema.sql)

---

## Táblák

### articles
| Mező           | Típus         | Constraint                                 |
|----------------|---------------|--------------------------------------------|
| id             | uuid          | PK, DEFAULT gen_random_uuid()              |
| author_id      | uuid          | FK → users.id ON DELETE SET NULL, NOT NULL |
| title          | text          | NOT NULL                                   |
| lead           | text          | NOT NULL                                   |
| is_summary     | boolean       | NOT NULL, DEFAULT false                    |
| slug           | text          | NOT NULL                                   |
| tag            | text          | NOT NULL                                   |
| first_section  | text          | NOT NULL                                   |
| second_section | text          | nullable                                   |
| third_section  | text          | nullable                                   |
| fourth_section | text          | nullable                                   |
| last_section   | text          | NOT NULL                                   |
| published_at   | timestamptz   | NOT NULL                                   |
| updated_at     | timestamptz   | NOT NULL                                   |

---

### brands
| Mező           | Típus  | Constraint                    |
|----------------|--------|-------------------------------|
| id             | uuid   | PK, DEFAULT gen_random_uuid() |
| name           | text   | NOT NULL                      |
| description    | text   | NOT NULL                      |
| principal      | text   | NOT NULL                      |
| headquarters   | text   | NOT NULL                      |

---

### circuits
| Mező         | Típus            | Constraint                    |
|--------------|------------------|-------------------------------|
| id           | uuid             | PK, DEFAULT gen_random_uuid() |
| name         | text             | NOT NULL                      |
| length       | double precision | NOT NULL                      |
| type         | text             | NOT NULL                      |
| location     | text             | NOT NULL                      |
| fastest_lap  | text             | NOT NULL                      |

---

### comments
| Mező         | Típus         | Constraint                                                  |
|--------------|---------------|-------------------------------------------------------------|
| id           | uuid          | PK, DEFAULT gen_random_uuid()                               |
| user_id      | uuid          | FK → users.id ON DELETE CASCADE, NOT NULL                   |
| article_id   | uuid          | FK → articles.id ON DELETE CASCADE, NOT NULL                |
| reply_id     | uuid          | FK → comments.id ON DELETE CASCADE, nullable (önreferencia) |
| content      | text          | NOT NULL                                                    |
| upvotes      | integer       | NOT NULL, DEFAULT 0                                         |
| downvotes    | integer       | NOT NULL, DEFAULT 0                                         |
| created_at   | timestamptz   |  NOT NULL                                                   |
| updated_at   | timestamptz   | NOT NULL                                                    |

> A `reply_id` önreferencia: threaded kommentek megvalósítása.

---

### constructors
| Mező            | Típus    | Constraint                    |
|-----------------|----------|-------------------------------|
| id              | uuid     | PK, DEFAULT gen_random_uuid() |
| brand_id        | uuid     | FK → brands.id, NOT NULL      |
| name            | text     | NOT NULL                      |
| nickname        | text     | NOT NULL                      |
| founded_year    | integer  | NOT NULL                      |
| headquarters    | text     | NOT NULL                      |
| team_chief      | text     | NOT NULL                      |
| technical_chief | text     | NOT NULL                      |
| seasons         | integer  | NOT NULL                      |
| championships   | integer  | NOT NULL                      |
| wins            | integer  | NOT NULL                      |
| podiums         | integer  | NOT NULL                      |

---

### contracts
| Mező           | Típus | Constraint                      |
|----------------|-------|---------------------------------|
| id             | uuid  | PK, DEFAULT gen_random_uuid()   |
| driver_id      | uuid  | FK → drivers.id, NOT NULL       |
| constructor_id |  uuid | FK → constructors.id, NOT NULL  |

> Egy vezető és egy konstruktőr aktuális szerződéses kapcsolatát rögzíti.

---

### constructor_champions
| Mező      | Típus  | Constraint                                 |
|-----------|--------|--------------------------------------------|
| id        | uuid   | PK, DEFAULT gen_random_uuid()              |
| series_id | uuid   | FK → series.id ON DELETE CASCADE, NOT NULL |
| name      | text   | NOT NULL                                   |
| season    | text   | NOT NULL                                   |
| status    | text   | NOT NULL                                   |

---

### constructor_competitions
| Mező                            | Típus  | Constraint                                    |
|---------------------------------|--------|-----------------------------------------------|
| constructors_id                 | uuid   | PK (composite), FK → constructors.id          |
| constructors_championship_id    | uuid   | PK (composite), FK → constructor_champions.id |
| constructor_name_snapshot       | text   | NOT NULL                                      |
| constructor_nickname_snapshot   | text   | NOT NULL                                      |

> Kapcsolótábla: konstruktőr ↔ bajnokság, snapshot mezőkkel a történeti névhez.

---

### comment_votes
| Mező       | Típus   | Constraint                                   |
|------------|---------|----------------------------------------------|
| id         | uuid    | PK, DEFAULT gen_random_uuid()                |
| user_id    | uuid    | FK → users.id ON DELETE SET NULL, nullable   |
| comment_id | uuid    | FK → comments.id ON DELETE CASCADE, NOT NULL |
| is_upvote  | boolean | NOT NULL                                     |

> UNIQUE index: (user_id, comment_id) – egy felhasználó egy kommentre csak egyszer szavazhat.

---

### drivers
| Mező           | Típus       | Constraint                      |
|----------------|-------------|---------------------------------|
| id             | uuid        | PK, DEFAULT gen_random_uuid()   |
| name           | text        | NOT NULL                        |
| nationality    | text        | NOT NULL                        |
| birth_date     | timestamptz | NOT NULL                        |
| total_races    | integer     | NOT NULL                        |
| wins           | integer     | NOT NULL                        |
| podiums        | integer     | NOT NULL                        |
| championships  | integer     | NOT NULL                        |
| pole_positions | integer     | NOT NULL                        |
| seasons        | integer     | NOT NULL                        |
---

### driver_championships
| Mező      | Típus | Constraint                                 |
|-----------|-------|--------------------------------------------|
| id        | uuid  | PK, DEFAULT gen_random_uuid()              |
| series_id | uuid  | FK → series.id ON DELETE CASCADE, NOT NULL |
| name      | text  | NOT NULL                                   |
| season    | text  | NOT NULL                                   |
| status    | text  | NOT NULL                                   |

---

### driver_participations
| Mező                    | Típus   | Constraint                                   |
|-------------------------|---------|----------------------------------------------|
| driver_id               | uuid    | PK (composite), FK → drivers.id              |
| drivers_championship_id | uuid    | PK (composite), FK → driver_championships.id |
| driver_number           | integer | NOT NULL, DEFAULT -1                         |
| driver_name_snapshot    | text    | NOT NULL                                     |

> Kapcsolótábla: vezető ↔ bajnokság. A `driver_name_snapshot` az akkori nevet rögzíti, névváltozás esetén is megőrzi a történeti adatot.

---

### grands_prix
| Mező               | Típus        | Constraint                                   |
|--------------------|--------------|----------------------------------------------|
| id                 | uuid         | PK, DEFAULT gen_random_uuid()                |
| circuit_id         | uuid         | FK → circuits.id, NOT NULL                   |
| series_id          | uuid         | FK → series.id ON DELETE CASCADE, NOT NULL   |
| name               | text         | NOT NULL                                     |
| short_name         | text         | NOT NULL                                     |
| round_number       | integer      | NOT NULL                                     |
| season_year        | integer      | NOT NULL                                     |
| start_time         | timestamptz  | NOT NULL                                     |
| end_time           | timestamptz  | NOT NULL                                     |
| race_distance      | integer      | NOT NULL                                     |
| laps_completed     | integer      | NOT NULL                                     |

---


### polls
| Mező        | Típus       | Constraint                                 |
|-------------|-------------|--------------------------------------------|
| id          | uuid        | PK, DEFAULT gen_random_uuid()              |
| author_id   | uuid        | FK → users.id ON DELETE SET NULL, nullable |
| title       | text        | NOT NULL                                   |
| tag         | text        | NOT NULL                                   |
| description | text        | NOT NULL                                   |
| created_at  | timestamptz | NOT NULL                                   |
| expires_at  | timestamptz | NOT NULL                                   |
| is_active   | boolean     | NOT NULL                                   |

---

### poll_options
| Mező    | Típus  | Constraint                                |
|---------|--------|-------------------------------------------|
| id      | uuid   | PK, DEFAULT gen_random_uuid()             |
| poll_id | uuid   | FK → polls.id ON DELETE CASCADE, NOT NULL |
| text    | text   | NOT NULL                                  |

---

### poll_votes
| Mező           | Típus  | Constraint                                               |
|----------------|--------|----------------------------------------------------------|
| user_id        | uuid   | PK (composite), FK → users.id ON DELETE SET NULL         |
| poll_option_id | uuid   | PK (composite), FK → poll_options.id ON DELETE CASCADE   |
| id             | uuid   | DEFAULT gen_random_uuid()                                |

> Egy felhasználó egy opcióra csak egyszer szavazhat (composite PK garantálja).

---

### qualifying_results
| Mező      | Típus    | Constraint                                |
|-----------|----------|-------------------------------------------|
| id        | uuid     | PK, DEFAULT gen_random_uuid()             |
| result_id | uuid     | FK → results.id ON DELETE CASCADE, UNIQUE |
| q1_time   | bigint   | nullable (ms)                             |
| q2_time   | bigint   | nullable (ms)                             |
| q3_time   | bigint   |  nullable (ms)                            |

> 1:1 kapcsolat a results táblával. Az időadatok milliszekundumban tároltak.

---

### results
| Mező                          | Típus            | Constraint                              |
|-------------------------------|------------------|-----------------------------------------|
| id                            | uuid             | PK, DEFAULT gen_random_uuid()           |
| grand_prix_id                 | uuid             | FK → grands_prix.id, NOT NULL           |
| driver_id                     | uuid             | FK → drivers.id, NOT NULL               |
| driver_name_snapshot          | text             | NOT NULL, DEFAULT ''                    |
| constructor_id                | uuid             | FK → constructors.id, NOT NULL          |
| constructor_name_snapshot     | text             | NOT NULL, DEFAULT ''                    |
| constructor_nickname_snapshot | text             | NOT NULL, DEFAULT ''                    |
| drivers_championship_id       | uuid             | FK → driver_championships.id, NOT NULL  |
| constructors_championship_id  | uuid             | FK → constructor_champions.id, NOT NULL |
| start_position                | integer          | NOT NULL                                |
| finish_position               | integer          | NOT NULL                                |
| session                       | text             | NOT NULL                                |
| car_number                    | integer          | NOT NULL                                |
| race_time                     | bigint           | NOT NULL (ms)                           |
| status                        | text             | NOT NULL                                |
| laps_completed                | integer          | NOT NULL                                |
| driver_points                 | double precision | NOT NULL                                |
| constructor_points            | double precision | NOT NULL                                |
| is_pole_position              | boolean          | NOT NULL                                |
| is_fastest_lap                | boolean          | NOT NULL                                |

---

### series
| Mező           | Típus   | Constraint                    |
|----------------|---------|-------------------------------|
| id             | uuid    | PK, DEFAULT gen_random_uuid() |
| name           | text    | NOT NULL                      |
| short_name     | text    | NOT NULL                      |
| description    | text    | NOT NULL                      |
| governing_body | text    | NOT NULL                      |
| first_year     | integer | NOT NULL                      |
| last_year      | integer | NOT NULL                      |
| point_system   | text    | NOT NULL                      |

---

### users
| Mező                | Típus          | Constraint                    |
|---------------------|----------------|-------------------------------|
| id                  | uuid           | PK, DEFAULT gen_random_uuid() |
| username            | varchar(50)    | NOT NULL                      |
| full_name           | text           | NOT NULL                      |
| email               | varchar(100)   | NOT NULL                      |
| password_hash       | text           | NOT NULL                      |
| role                | varchar(20)    | NOT NULL, DEFAULT 'user'      |
| created_at          | timestamptz    | NOT NULL                      |
| has_avatar          | boolean        | NOT NULL, DEFAULT false       |
| last_active         | timestamptz    | nullable                      |
| last_login          | timestamptz    | nullable                      |
| is_logged_in        | boolean        | NOT NULL                      |
| current_session_id  | varchar(100)   | nullable                      |

---

## Kapcsolatok összefoglalója

| Kapcsolat                            | Típus | Megjegyzés                                  |
|--------------------------------------|-------|---------------------------------------------|
| brands → constructors                | 1:N   | Egy brand több konstruktőrhöz tartozhat     |
| drivers ↔ constructors               | N:M   | `contracts` kapcsolótáblán keresztül        |
| series → driver_championships        | 1:N   | ON DELETE CASCADE                           |
| series → constructor_champions       | 1:N   | ON DELETE CASCADE                           |
| series → grands_prix                 | 1:N   | ON DELETE CASCADE                           |
| circuits → grands_prix               | 1:N   |                                             |
| drivers ↔ driver_championships       | N:M   | `driver_participations` kapcsolótábla       |
| constructors ↔ constructor_champions | N:M   | `constructor_competitions` kapcsolótábla    |
| grands_prix → results                | 1:N   |                                             |
| results → qualifying_results         | 1:1   | ON DELETE CASCADE                           |
| users → articles                     | 1:N   | ON DELETE SET NULL                          |
| users → comments                     | 1:N   | ON DELETE CASCADE                           |
| articles → comments                  | 1:N   | ON DELETE CASCADE                           |
| comments → comments                  | 1:N   | Önreferencia (reply_id), threaded kommentek |
| comments → comment_votes             | 1:N   | ON DELETE CASCADE                           |
| users → polls                        | 1:N   | ON DELETE SET NULL                          |
| polls → poll_options                 | 1:N   | ON DELETE CASCADE                           |
| poll_options → poll_votes            | 1:N   | ON DELETE CASCADE                           |

---

## Indexek

| Index                                                    | Tábla                    | Típus    |
|----------------------------------------------------------|--------------------------|----------|
| IX_articles_author_id                                    | articles                 | B-tree   |
| IX_comments_user_id                                      | comments                 | B-tree   |
| IX_comments_article_id                                   | comments                 | B-tree   |
| IX_comments_reply_id                                     | comments                 | B-tree   |
| IX_comment_votes_comment_id                              | comment_votes            | B-tree   |
| IX_comment_votes_user_id_comment_id                      | comment_votes            | UNIQUE   |
| IX_results_grand_prix_id                                 | results                  | B-tree   |
| IX_results_driver_id                                     | results                  | B-tree   |
| IX_results_constructor_id                                | results                  | B-tree   |
| IX_results_drivers_championship_id                       | results                  | B-tree   |
| IX_results_constructors_championship_id                  | results                  | B-tree   |
| IX_qualifying_results_result_id                          | qualifying_results       | UNIQUE   |
| IX_constructors_brand_id                                 | constructors             | B-tree   |
| IX_contracts_driver_id                                   | contracts                | B-tree   |
| IX_contracts_constructor_id                              | contracts                | B-tree   |
| IX_grands_prix_circuit_id                                | grands_prix              | B-tree   |
| IX_grands_prix_series_id                                 | grands_prix              | B-tree   |
| IX_driver_championships_series_id                        | driver_championships     | B-tree   |
| IX_constructor_champions_series_id                       | constructor_champions    | B-tree   |
| IX_driver_participations_drivers_championship_id         | driver_participations    | B-tree   |
| IX_constructor_competitions_constructors_championship_id | constructor_competitions | B-tree   |
| IX_polls_author_id                                       | polls                    | B-tree   |
| IX_poll_options_poll_id                                  | poll_options             | B-tree   |
| IX_poll_votes_poll_option_id                             | poll_votes               | B-tree   |