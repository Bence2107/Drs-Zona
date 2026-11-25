# Workflow, git and style

## Git conventions

From `CONTRIBUTING.md`, enforced by `.github/workflows/pr-policy.yml`:

- Branch: `<type>/drs-<number>-<short-name>`, types `feature fix refactor perf test docs style build ci chore`. Example: `fix/drs-57-login-redirect`.
- PR title and commit messages: `<type>(<scope>): <Subject> (DRS-<number>)`, types `feat fix refactor perf test docs style build ci chore revert`. Example: `feat(polls): Show live results (DRS-42)`.
- PRs are merged with a merge commit, so every commit lands in `master` as written. Keep each one focused and its message meaningful.
- Fill in the PR template, including **Upgrade notes** (new env variables, migrations, tools).
- Do not bypass the pre-commit hook with `--no-verify` to get a commit through.

## Code and docs style

- **Comments explain why, never what.** Comment a constraint, an edge case or a decision that the code cannot show. No comments or XML docs that restate the code.
- **Language:** code, comments, commits and docs are in English. Everything the user sees (UI text, validation and error messages from the API) is in Hungarian.
- **READMEs describe local development only.** The repository is public: never add production server details (hosts, service units, deploy paths, credentials) to any file in it.
- Keep `README.md`, `server/README.md` and `client/README.md` in sync when you change setup, scripts, configuration or ports.

## Definition of done

Run what applies to the change, from the repository root:

```bash
# server
dotnet format server/Drs-Zona.sln --verify-no-changes
dotnet test server/Drs-Zona.sln

# client
cd client
pnpm format:check
pnpm lint
pnpm test
pnpm build
```

A server change that alters an endpoint, a DTO or a route also needs a regenerated client (`pnpm open-api-gen`) in the same PR, and the client build must still pass.
