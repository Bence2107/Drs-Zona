# Drs-Zóna

A motorsport community portal: an Angular 21 client (`client/`) and an ASP.NET Core 9 API with PostgreSQL (`server/`). One API process also serves the built client in production.

The rules for working on this codebase are in [`.claude/rules/`](.claude/rules/). Claude Code loads them automatically: the general ones in every session, the server and client ones when you work on matching files. They exist because the codebase was built quickly with AI help and its structure is easy to erode one "small improvement" at a time. When a rule and a request conflict, stop and ask. Do not work around a rule.

## How to work here

1. **Every change belongs to a ticket** (`DRS-<number>`). If the request has none, ask for it before committing.
2. **Do only what the ticket asks.** No drive-by refactors, renames, reformatting of untouched files or "while I'm here" fixes. Mention them at the end instead.
3. **One concern per commit.** Split larger work into stages that each build and pass tests on their own. Stop after each stage so the user can review and commit; do not run `git commit` or `git push` unless asked.
4. **Follow the existing pattern of the area you touch.** If you think the pattern is wrong, say so and propose a separate ticket. Do not introduce a second way of doing the same thing.
5. **Verify before you say it is done** (see `.claude/rules/workflow-and-git.md`). Report failing checks as they are.

## Architecture at a glance

```text
Angular page/component
  → client/src/app/services/api/<area>.service.ts     (wraps the generated client)
  → client/src/app/api/fn/...                         (generated, relative /api URLs)
  → [HTTP] API controller (server/API/Controllers)    (thin: auth, user id, HTTP mapping)
  → Service (server/Services)                         (business rules, returns ResponseResult<T>)
  → Repository (server/Repositories)                  (EF Core queries per entity)
  → EfContext (server/Context) → PostgreSQL
```

Entities never leave the server; services map them to DTO records (`server/DTOs`). The client sees only DTOs, through the generated models.

## Rules

| File | Loaded | Covers |
| --- | --- | --- |
| `protected-areas.md` | Always | What not to change without explicit approval, and what to do instead |
| `workflow-and-git.md` | Always | Git conventions, code and docs style, definition of done |
| `tooling-decisions.md` | Always | The DRS-26 tooling choices and why they stay; new settings default to today's behaviour |
| `modernisation.md` | Always | How new patterns enter the codebase without a rewrite |
| `server-architecture.md` | `server/**` | Layers, adding a feature, services, controllers, server debt |
| `server-auth.md` | Controllers, auth code, client guards | Policies, roles, JWT |
| `server-data.md` | Entities, Context, Repositories | Entities, repositories, migrations |
| `server-config.md` | Configuration, `Program.cs`, `.env.example` | Environment variables, `.env` lookup, uploads |
| `server-tests.md` | `server/Tests/**` | xUnit, Moq, in-memory database |
| `client-architecture.md` | `client/**` | Data flow, components, UI, locale, dependencies, client debt |
| `client-api.md` | Generated client and API services | ng-openapi-gen, regeneration, relative URLs |
| `client-tests.md` | `client/**/*.spec.ts` | Vitest |

## Local commands

```bash
cp .env.example .env                       # once; fill in DB_PASSWORD and JWT_SECRET
docker compose up -d                       # local PostgreSQL
cd server/Context && dotnet ef database update
cd server/API && dotnet run --launch-profile https   # API, Swagger at http://localhost:5244/swagger
cd client && pnpm install && pnpm start              # client at http://localhost:4200
```

Only ever run `dotnet ef database update` against the local database.
