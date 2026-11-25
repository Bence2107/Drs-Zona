---
paths:
  - "server/Entities/**"
  - "server/Context/**"
  - "server/Repositories/**"
---

# Entities, repositories and migrations

## Entities

- One class per table in `Entities/Models/<Domain>/`.
- Table and column names in snake_case with `[Table]` and `[Column]`.
- Navigation properties are `virtual` and `[JsonIgnore]`.
- Relationships and delete behaviour go in `EfContext.OnModelCreating`, under the domain's comment block. Add the `DbSet` to `EfContext`.
- Entities never leave the server. Services map them to DTOs.

## Repositories

- Interface in `Repositories/Interfaces/<Domain>/I<Name>Repository.cs`, implementation in `Repositories/Implementations/<Domain>/`.
- Primary constructor taking `EfContext`. Async EF queries only; return entities or primitives, never DTOs.
- No business rules here, and no generic base repository.

## Migrations

- Never edit, rename or delete an existing migration or `EfContextModelSnapshot.cs` by hand. They are applied to a database with real data.
- After a model change: `cd server/Context && dotnet ef migrations add <PascalCaseName>`, one migration per logical schema change.
- Read the generated `Up` and `Down` before committing. A renamed property generates a drop and add, which loses data; fix that in the new migration (`RenameColumn`), not in the model snapshot.
- Prefer additive changes: new nullable columns or columns with defaults. Dropping or narrowing a column needs the user's explicit approval.
- Migrations create schema only; do not add seed data to them.
- Only run `dotnet ef database update` against the local Docker database.
- Mention every new migration in the PR's **Upgrade notes**.
