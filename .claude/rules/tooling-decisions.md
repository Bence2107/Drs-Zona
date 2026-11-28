# Tooling decisions (DRS-26) and why they stay

These were deliberate choices. Do not switch them back or add an alternative next to them.

| Decision | Rule |
| --- | --- |
| **pnpm** for the client (replaced npm) | Never use `npm` or `yarn`, never add `package-lock.json` or `yarn.lock`. `engines.pnpm` is `>=12`. |
| **Vitest** for client tests (replaced Karma/Jasmine) | Use `vi.fn()` and `vi.spyOn()`; no Jasmine APIs, no `fakeAsync`. |
| **Prettier + ESLint** (angular-eslint) for the client | Both run in CI and in the pre-commit hook. |
| **simple-git-hooks + lint-staged** pre-commit hook | Installed by `pnpm install`; fixes and lints staged client files. |
| **`dotnet format`** for the server | CI runs `dotnet format server/Drs-Zona.sln --verify-no-changes`. |
| **One `.env` in the repo root** for API, `dotnet ef`, tests and client dev scripts | New settings are environment variables documented in `.env.example`. No `appsettings.*.json` secrets, no `environment.ts` files. |
| **Docker Compose only for the local database** | `docker-compose.yml` is for development; the API is not containerised. |
| **Optional ports** (`FRONTEND_PORT`, `API_PORT`, `API_HTTPS_PORT`) and **uploads folder** (`UPLOADS_PATH`) | Unset means the old behaviour. Dev-only variables are ignored outside Development. |
| **SkiaSharp** for image processing (replaced ImageSharp, DRS-25) | Do not reintroduce ImageSharp. |
| **CI in three stages**: check → test → build | All must be green before merge. |
| **PR policy** workflow | Branch and PR title formats are enforced (see `workflow-and-git.md`). |

## New settings must default to today's behaviour

Every new configuration value is optional. When it is not set, the application behaves exactly as before, so production keeps working without a server change. In `.env.example`, add it commented out with its default and say who reads it. Add it to the variable table in `server/README.md`.
