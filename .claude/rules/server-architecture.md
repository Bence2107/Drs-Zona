---
paths:
  - "server/**"
---

# Server architecture (ASP.NET Core 9, EF Core, PostgreSQL)

## Layers and dependency direction

The project references are the architecture. Do not add a `ProjectReference` or move a class to a project that would let a lower layer see a higher one.

```text
API ──→ Services ──→ Repositories ──→ Context ──→ Entities
 │          │
 └──────────┴──→ DTOs
```

| Project | Owns | Must not |
| --- | --- | --- |
| `API` | Controllers, DI registration, auth, CORS, static files (`Program.cs`) | Contain business rules, query `EfContext`, or use repositories (they are referenced only for DI registration) |
| `Services` | Business rules, ownership checks, entity-to-DTO mapping, images, JWT, password hashing | Return entities to the API, use `HttpContext` or ASP.NET MVC types |
| `Repositories` | EF Core queries, one repository per entity, behind an interface | Return DTOs, contain business rules, call services |
| `Context` | `EfContext`, migrations, `.env` loading, connection string | Reference anything except `Entities` |
| `Entities` | EF entity classes and `JwtSettings` | Reference other projects |
| `DTOs` | Request and response records with validation attributes | Contain logic beyond input validation |
| `Tests` | Unit, integration and controller tests | Be referenced by anything |

`DTOs` currently references `Repositories`. That is legacy; do not use it for anything new.

## Adding a feature: the path through the layers

Follow an existing slice, for example polls: `PollsController` → `IPollService`/`PollService` → `IPollsRepository`/`PollsRepository` → `Poll` entity, with `PollsDTOs.cs`.

1. **Entity and migration**: see `server-data.md`.
2. **Repository**: see `server-data.md`.
3. **DTOs**: records in `DTOs/<Domain>/<Domain>DTOs.cs`. Validate input with data annotations and `IValidatableObject`; error messages in Hungarian.
4. **Service**: interface in `Services/Interfaces/`, implementation in `Services/Implementations/`. Every public method returns `Task<ResponseResult<T>>`. Map entities to DTOs by hand (no AutoMapper).
5. **Register** the repository and service in `Program.cs` with `AddScoped`, next to the others of the same domain.
6. **Controller** in `API/Controllers/`. See [Controllers](#controllers).
7. **Tests**: unit and integration tests for the service (see `server-tests.md`).
8. **Client**: start the API and run `pnpm open-api-gen` in `client/`; commit the regenerated files in the same PR.

## Services and `ResponseResult<T>`

- Expected failures (not found, not allowed, invalid state) return `ResponseResult<T>.Failure(message)` or `Failure(field, message)`. Do not throw for them.
- Messages are user-facing and in Hungarian, like the existing ones (`"Szavazás nem található"`).
- Ownership and state checks (is this the author, is the poll still open) live in the service, not in the controller.
- Repositories save their own changes today (`SaveChangesAsync` inside `Add`, `Delete`, ...). When one operation spans several repositories, the service opens a transaction on `EfContext` around them, as `PollService.Create` does. That is the only reason a service may use `EfContext` directly.
- No generic repository, Unit of Work, MediatR or CQRS layer unless a ticket introduces it for the whole server.

## Controllers

- `[ApiController]`, `[Route("api/[controller]")]`, primary constructor injecting service interfaces only.
- Action routes follow the controller's existing style (`get/{id:guid}`, `create`, `update`, `delete/{id:guid}`). Do not rename or remove existing routes: the generated client and the deployed build call them.
- Read the user id from `ClaimTypes.NameIdentifier`; return `Unauthorized()` when it is missing on an `[Authorize]` action.
- Map `ResponseResult` to HTTP in the controller: success → `Ok(value)`, failure → `BadRequest(...)` (or `NotFound` where the controller already does so).
- Error bodies are a contract with `client/src/app/services/error-interceptor.service.ts`, which maps `{ field, message }` and ASP.NET validation `errors` to form fields. `AuthController` sends `{ field, message }`; the other controllers send `{ ErrorField, Message }`, which the client does not map to a field. Do not change either side alone; fixing this needs a ticket that changes both.

## Style

- `dotnet format` with the .NET defaults; CI fails on any difference.
- Nullable reference types are on in every project; do not disable them or add `!` to silence a real null case.
- File-scoped namespaces and primary constructors, as in the existing code.

## Known debt (do not add more, do not fix unasked)

- Error body shape differs between `AuthController` and the other controllers (see [Controllers](#controllers)).
- `DateTime.Now` and `DateTime.UtcNow` are mixed in services and repositories. Do not change existing comparisons without a ticket; they affect stored data.
- `IPollVotesRepository` is registered twice in `Program.cs`, and `UseSwagger` is called twice.
- `CORS_ORIGINS` is read with `Environment.GetEnvironmentVariable` instead of `EnvVars`.
- Some file and type names carry typos (`ConstructorCompition.cs`, `GrandRrixResultDto`, `IConstuctorsChampionshipsRepository.cs`). Renaming them changes tables, routes or generated client files, so it needs its own ticket.
