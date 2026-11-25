---
paths:
  - "server/Context/Configuration/**"
  - "server/API/Program.cs"
  - "server/API/Properties/**"
  - "server/Services/Types/UploadSettings.cs"
  - "server/Services/**/images/**"
  - ".env.example"
  - "docker-compose.yml"
  - "client/scripts/**"
  - "client/proxy.conf.mjs"
---

# Configuration, startup and uploads

## Environment variables

- Read settings only through `Context.Configuration.EnvVars` (`Get`, `GetOptional`, `GetOptionalInt`, `Require`). Do not use `IConfiguration`, `appsettings.json` or `Environment.GetEnvironmentVariable` for new settings.
- A new variable is optional and defaults to today's behaviour. Dev-only variables are read inside `if (builder.Environment.IsDevelopment())`; production takes its port from `ASPNETCORE_URLS`.
- Document every new variable in `.env.example` (commented out, with its default and who reads it) and in the table in `server/README.md`.
- Do not change the `.env` lookup order in `EnvFile.cs`: explicit `ENV_FILE`, then the nearest `.env` up to the repository root, then `server/API/.env`. The client's `scripts/load-env.mjs` follows the same rules for `ENV_FILE`.
- Required variables fail at startup with one message listing all that are missing (`EnvVars.Require`). Keep it that way for new required variables.

## `Program.cs`

- The content root, the client `dist` path, the static file setup, the HTTPS redirect (Development only), Swagger (Development only) and the middleware order are load-bearing for production. Do not change them.
- New registrations go next to the existing group for their domain.

## Uploads and images

- Uploaded files live under `UploadSettings.RootPath` (from `UPLOADS_PATH`, default `uploads` in the content root) and are served at `/uploads`. Never hard-code a path, never write to `wwwroot` or the source tree.
- Image processing uses SkiaSharp through the services in `Services/Implementations/images/`. Do not add another imaging library.

## Local database

- `docker-compose.yml` runs PostgreSQL 17 for development only, from the same `.env` values, published on `127.0.0.1`. Keep it that way.
