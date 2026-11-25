# Drs-Zóna API

ASP.NET Core 9 Web API for Drs-Zóna. It serves the REST API, user uploads and, in production, the built Angular client.

## Requirements

- .NET 9 SDK
- PostgreSQL 15 or newer, or Docker to run the database from the root [`docker-compose.yml`](../docker-compose.yml) (PostgreSQL 17)
- The EF Core CLI, for migrations:

  ```bash
  dotnet tool install --global dotnet-ef --version 9.*
  ```

## Project layout

The solution is [`Drs-Zona.sln`](Drs-Zona.sln). Every project targets `net9.0`.

| Project | Responsibility |
| --- | --- |
| `API` | Entry point: controllers, dependency injection, authentication, CORS and static files (`Program.cs`) |
| `Services` | Business logic, image processing (SkiaSharp), JWT issuing and password hashing (BCrypt) |
| `Repositories` | Data access per domain, behind interfaces |
| `Context` | `EfContext`, migrations and configuration loading (`.env`, database connection) |
| `Entities` | EF Core entity models |
| `DTOs` | Request and response contracts |
| `Tests` | Unit, integration and controller tests |

```text
server/
├── Drs-Zona.sln
├── API/
│   ├── Controllers/           One controller per area
│   ├── Properties/            Launch profiles (ports)
│   └── Program.cs             Service registration and request pipeline
├── Context/
│   ├── Configuration/         .env loading, environment variables, connection string
│   ├── Migrations/            EF Core migrations
│   ├── EfContext.cs
│   └── EfContextFactory.cs    Used by dotnet ef
├── DTOs/                      Auth/, News/, Polls/, RaceTracks/, Standings/
├── Entities/
│   ├── Extensions/            JwtSettings
│   └── Models/                News/, Polls/, RaceTracks/, Standings/, User.cs
├── Repositories/
│   ├── Interfaces/            Grouped by domain like Models/
│   └── Implementations/
├── Services/
│   ├── Interfaces/
│   ├── Implementations/       images/ holds the upload services
│   └── Types/                 ResponseResult, PagedResult, UploadSettings
└── Tests/
    ├── Services/Units/        Services with mocked repositories
    ├── Services/Integrations/ Services on an in-memory database
    ├── Controllers/           HTTP tests through WebApplicationFactory
    └── InMemoryDbFactory.cs
```

## Running locally

Create the `.env` file and start the database first (see [Configuration](#configuration) and [Database and migrations](#database-and-migrations)). Then run the API from `server/API`:

```bash
cd server/API
dotnet run --launch-profile https
```

| Launch profile   | URLs                                                 |
| ---------------- | ---------------------------------------------------- |
| `https`          | `https://localhost:7221` and `http://localhost:5244` |
| `http` (default) | `http://localhost:5244`                              |

The Angular dev server sends `/api` to the HTTPS port and `/uploads` to the HTTP port, so use the `https` profile when you work on the client.

| Endpoint | Purpose |
| --- | --- |
| `/swagger` | Swagger UI, Development only |
| `GET /api/healthcheck/ping` | Returns `{ "status": "Healthy" }` if the database is reachable, otherwise `500` |

To use other ports, set `API_PORT` and `API_HTTPS_PORT` in `.env`. They apply in Development only. In production the port comes from `ASPNETCORE_URLS`.

## Configuration

All settings are environment variables, usually kept in a `.env` file in the repository root. Start from the template:

```bash
cp .env.example .env
```

| Variable | Required | Default | Purpose |
| --- | --- | --- | --- |
| `DB_HOST` | yes | | PostgreSQL host |
| `DB_NAME` | yes | | Database name |
| `DB_USER` | yes | | Database user |
| `DB_PASSWORD` | no | | Database password |
| `DB_PORT` | no | `5432` | PostgreSQL port |
| `JWT_SECRET` | yes | | Token signing key, at least 32 bytes |
| `JWT_ISSUER` | no | `MotorsportSocialNetwork` | Token issuer |
| `JWT_AUDIENCE` | no | `MotorsportSocialNetworkUsers` | Token audience |
| `JWT_EXPIRATION_MINUTES` | no | `1440` | Token lifetime |
| `CORS_ORIGINS` | no | `http://localhost:4200` | Allowed origins, comma-separated |
| `UPLOADS_PATH` | no | `uploads` in the content root | Upload folder; a relative path starts from the content root |
| `API_PORT` | no | `5244` | HTTP port, Development only |
| `API_HTTPS_PORT` | no | `7221` | HTTPS port, Development only |

The API, `dotnet ef` and the tests all look for the file in this order ([`Context/Configuration/EnvFile.cs`](Context/Configuration/EnvFile.cs)):

1. The path in `ENV_FILE`. Startup fails if that file does not exist.
2. The nearest `.env` walking up from the working directory to the repository root.
3. `server/API/.env`, the legacy location.

Variables already set in the environment take precedence over the file. If a required variable is missing, the API does not start and the error lists all missing variables at once. The log shows which file was loaded (`Loaded environment from …`).

`.env` is gitignored. Never commit it.

## Authentication and roles

The API uses JWT bearer tokens signed with HS256. `POST /api/auth/login` returns a token, and the client sends it in the `Authorization: Bearer <token>` header. Passwords are hashed with BCrypt.

| Role | Can do |
| --- | --- |
| `User` | Comment, vote in polls, manage own profile |
| `Author` | Everything a user can, plus create, edit and delete articles |
| `Editor` | Everything a user can, plus manage championships, drivers, constructors, contracts, Grands Prix and results |
| `Admin` | Everything |

Endpoints are protected with `[Authorize]` (any signed-in user) or with one of two policies: `AuthorOrAdmin` and `EditorOrAdmin`. Registration always creates a `User`, so other roles must be assigned in the database.

## API overview

Routes follow `api/<controller>/<action>`. Swagger lists every endpoint with its request and response types.

| Route prefix | Area | Access |
| --- | --- | --- |
| `api/auth` | Registration, login, profile, password, profile picture | Register and login public, everything else signed in |
| `api/article` | Articles | Read public, write `AuthorOrAdmin` |
| `api/article-image` | Article image upload, draft images | `AuthorOrAdmin` |
| `api/comments` | Comments and comment votes | Read public, write signed in |
| `api/poll` | Polls and votes | Read public, write signed in |
| `api/series` | Racing series | Public |
| `api/standings` | Results and standings | Read public, write `EditorOrAdmin` |
| `api/championship` | Championships and participations | Read public, write `EditorOrAdmin` |
| `api/grandprix` | Grands Prix and circuits | Mostly public; circuits list and writes `EditorOrAdmin` |
| `api/drivers` | Drivers | `EditorOrAdmin` |
| `api/constructors` | Constructors | `EditorOrAdmin` |
| `api/contracts` | Driver contracts | `EditorOrAdmin` |
| `api/brands` | Constructor brands | `EditorOrAdmin` |
| `api/healthcheck` | Health check | Public |

## File uploads

Uploaded images are stored under `uploads/` in the content root (`server/API/uploads` with `dotnet run`) and served at `/uploads`. Set `UPLOADS_PATH` to keep them elsewhere. The folder is created at startup if it does not exist.

| Subfolder | Content |
| --- | --- |
| `images/avatars/<userId>/` | Profile pictures |
| `images/articles/<slug>/` | Images of published articles |
| `dumps/articles/<draftId>/` | Images of unsaved drafts; moved to `images/articles/` when the article is published |
| `images/circuits/<id>/` | Circuit layouts |

## Database and migrations

Migrations live in [`Context/Migrations`](Context/Migrations). Run `dotnet ef` from `server/Context`. It reads the same `.env` as the API, so no connection string is needed.

```bash
cd server/Context

dotnet ef database update                         # apply all migrations
dotnet ef migrations list                         # show applied and pending migrations
dotnet ef migrations add <Name>                   # create a migration after a model change
dotnet ef database update <PreviousMigration>     # roll back to an earlier migration
```

The migrations create the schema only and include no seed data.

### Local database

The root [`docker-compose.yml`](../docker-compose.yml) runs PostgreSQL 17 for development. It creates the database from the `DB_NAME`, `DB_USER`, `DB_PASSWORD` and `DB_PORT` values in `.env`, so the API and `dotnet ef` connect to it without extra settings. The port is published on `127.0.0.1` only.

```bash
docker compose up -d          # start (from the repository root)
docker compose ps             # status; the container should be "healthy"
docker compose logs -f db     # follow the logs
docker compose down           # stop, keeping the data
docker compose down -v        # stop and delete the database
```

- If port 5432 is already in use, set `DB_PORT` to another port, for example `5433`. The container and the API both follow it.
- `DB_NAME`, `DB_USER` and `DB_PASSWORD` only take effect when the data volume is first created. After changing them, recreate the database with `docker compose down -v`, which deletes all data.
- With the `.env` somewhere else, pass it explicitly: `docker compose --env-file <path> up -d`.
- To copy data from an existing local PostgreSQL (here on 5432, with the container on 5433):

  ```bash
  pg_dump -h localhost -p 5432 -U postgres -Fc drs_zona > drs_zona.dump
  pg_restore -h localhost -p 5433 -U postgres -d drs_zona --no-owner drs_zona.dump
  ```

## Tests

The tests use xUnit, Moq and FluentAssertions. They run against the EF Core in-memory provider, so no database is needed.

```bash
cd server
dotnet test
```

| Suite | Namespace | Scope |
| --- | --- | --- |
| Unit | `Tests.Services.Units` | Services with mocked repositories |
| Integration | `Tests.Services.Integrations` | Services with real repositories on an in-memory database |
| Controller | `Tests.Controllers` | HTTP requests through `WebApplicationFactory` in the `Testing` environment |

Run one suite:

```bash
dotnet test --filter "FullyQualifiedName~Tests.Services.Units"
```

To write a results file or collect coverage:

```bash
dotnet test --logger "trx;LogFileName=test_results.trx"
dotnet test --collect:"XPlat Code Coverage"
```

`JWT_SECRET` must be set for the controller tests, either in the environment or in `.env`. The test host replaces the JWT settings afterwards, but startup checks the variable first.
