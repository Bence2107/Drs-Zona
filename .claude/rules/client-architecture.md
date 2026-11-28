---
paths:
  - "client/**"
---

# Client architecture (Angular 21, Angular Material, pnpm)

Run every client command from `client/`.

## Data flow

```text
pages/ or components/
  → services/api/<area>.service.ts   one service per API area, providedIn: 'root'
  → api/functions (generated)        e.g. apiPollGetIdGet$Json(http, rootUrl, params)
  → HttpClient + interceptors        authInterceptor adds the JWT, errorInterceptor normalises errors
```

- Components and pages never import from `api/fn` or `api/functions`, and never inject `HttpClient`. They call a service in `services/api/` (see `client-api.md`).
- Types from `api/models/` may be imported anywhere. Do not redeclare them; UI-only shapes go in `models/`.

## Errors, loading and empty states

- HTTP errors arrive as `HttpValidationError` (`{ title, fieldErrors }`) from `errorInterceptor`. Show field errors with `FormErrorService.applyServerErrors`. Do not parse `HttpErrorResponse` in components.
- Use the shared `app-loading-state` and `app-error-display` components (unified in DRS-24) and keep layouts from shifting while data loads. Do not add page-specific spinners.
- User notifications go through the existing `custom-snackbar` component.

## Routing

- Editor pages live under `pages/admin` and the `admin` route with `editorGuard`; article editing uses `authorGuard`. Keep new admin pages in that tree. Guards are UX only (see `server-auth.md`).

## Components: write new code the modern way

These patterns apply to new code and to files the ticket already changes. Do not convert untouched files.

| Use | Instead of |
| --- | --- |
| Standalone components | `NgModule` (there are none; keep it that way) |
| `inject()` | Constructor injection (most existing code; ESLint warns via `prefer-inject`) |
| `signal`, `computed`, `toSignal` for component state | Mutable fields plus manual subscriptions |
| Built-in control flow `@if`, `@for`, `@switch` | `*ngIf`, `*ngFor` (already removed everywhere) |
| Functional guards and interceptors | Class-based ones |
| `takeUntilDestroyed()` or `toSignal` for subscriptions | Unmanaged `subscribe()` calls |

- Selector prefix `app`, kebab-case elements; component styles in `.scss`.
- Shared UI goes in `components/`, routed screens in `pages/`, page-only parts in `pages/<page>/components/`, page state services next to the page (see `pages/standings/components/standings-state.service.ts`).
- Angular Material is the UI kit. Do not add another component library, CSS framework or icon set.
- Light and dark theme colours are CSS variables in `src/styles.scss`. Use them; do not hard-code colours. The inline script in `src/index.html` applies the saved theme before the first paint to avoid a white flash; keep it.

## Language and locale

- UI text is Hungarian. The app locale is `hu-HU` and the paginator labels come from `shared/getHungarianPaginator.ts`.
- Format dates and numbers with Angular pipes so they follow the locale.

## Dependencies

- pnpm only. Add or remove packages with `pnpm add` / `pnpm remove`; never edit `pnpm-lock.yaml` by hand.
- A package that needs an install script must be allowed in `pnpm-workspace.yaml` (`allowBuilds`). Add it there on purpose, not to silence a warning.
- Security overrides live in `pnpm-workspace.yaml` (`overrides`). Do not remove one without checking the advisory it fixes.
- No new runtime dependency without a ticket that asks for it. Prefer the Angular platform and RxJS.

## Style and checks

- Prettier: 100 columns, single quotes, Angular parser for templates. ESLint: angular-eslint recommended, template accessibility, typescript-eslint recommended and stylistic.
- The rules set to `warn` in `eslint.config.js` are existing debt. Do not add new warnings. Do not add `eslint-disable` comments or switch rules to `warn` or `off` to pass CI.
- TypeScript runs in `strict` mode with `strictTemplates`. Do not loosen `tsconfig*.json`, and do not use `any` or non-null assertions to silence a real type error.
- The production budget in `angular.json` (3 MB initial) is a limit, not a target. Do not raise it to make a build pass.

## Known debt (do not add more, do not fix unasked)

- Constructor injection in most services and components.
- `any` in some services, e.g. `PollService.getAllActive`.
- `provideHttpClient` and `provideRouter` are each registered more than once in `app.config.ts`, and an `ErrorHandler` provider has no implementation. Changing provider order changes interceptor behaviour, so fix it only in a dedicated ticket.
- Accessibility warnings in templates (click handlers without key events, labels without controls).
