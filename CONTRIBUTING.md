# Contributing

## Workflow

1. Every change belongs to a ticket with an ID like `DRS-42`. Report bugs and request features with the [issue templates](https://github.com/Bence2107/Drs-Zona/issues/new/choose).
2. Create a branch from `master`, named as described below.
3. Commit your changes. The pre-commit hook formats and lints the staged client files.
4. Open a pull request against `master` and fill in the template.
5. Both workflows must pass: **CI** (format, lint, tests, build) and **PR policy** (branch name and PR title).
6. Pull requests are merged with a merge commit, not squashed, so every commit ends up in `master`. Keep commits focused and their messages meaningful.

## Branch names

```text
<type>/drs-<number>-<short-name>
```

- `type`: one of the branch types below
- `drs-<number>`: the ticket ID, in lowercase
- `short-name`: a few words in lowercase, separated by hyphens

| Type | Use for |
| --- | --- |
| `feature` | New functionality |
| `fix` | Bug fixes |
| `refactor` | Code changes that neither fix a bug nor add a feature |
| `perf` | Performance improvements |
| `test` | Adding or fixing tests |
| `docs` | Documentation only |
| `style` | Formatting only, no behaviour change |
| `build` | Dependencies and build configuration |
| `ci` | CI workflows |
| `chore` | Other maintenance |

Examples: `feature/drs-42-poll-results`, `fix/drs-57-login-redirect`, `chore/drs-26-refactor-tech-choices`.

## Pull request titles

```text
<type>(<scope>): <subject> (DRS-<number>)
```

- `type`: `feat`, `fix`, `refactor`, `perf`, `test`, `docs`, `style`, `build`, `ci`, `chore` or `revert`. These are the branch types, except that new functionality is `feat` instead of `feature`, and `revert` is title-only.
- `scope`: optional, in lowercase, for the area that changed, e.g. `client`, `server`, `polls`.
- `!` after the type or scope marks a breaking change: `feat(api)!: …`.
- `subject`: a short summary of the change.
- `(DRS-<number>)`: the ticket ID, in uppercase.

Examples:

```text
feat(polls): Show live results (DRS-42)
fix(server): Reject expired tokens (DRS-57)
chore: Rework local setup, tooling and CI (DRS-26)
```

Use the same format for commit messages. They are not checked, but they end up in `master` unchanged.

## Code style

Both styles are checked in CI, so fix them before pushing.

- **Client:** Prettier and ESLint. The pre-commit hook fixes staged files; to check everything, run `pnpm format` and `pnpm lint` in `client/`. See the [client README](client/README.md#pre-commit-hook).
- **Server:** `dotnet format` with the .NET defaults. Run `dotnet format` in `server/`. See the [server README](server/README.md#formatting).
