---
paths:
  - "client/**/*.spec.ts"
  - "client/tsconfig.spec.json"
---

# Client tests

- Vitest through the Angular CLI (`pnpm test`, `pnpm test:watch`, `pnpm test --coverage`), in Node with jsdom; no browser needed.
- Specs sit next to the file they test as `*.spec.ts`.
- Use `vi.fn()`, `vi.spyOn()` and `async` tests with `await fixture.whenStable()`. No Jasmine APIs (`jasmine.createSpyObj`, `spyOn`), no `fakeAsync`.
- Mock services from `services/api/`, not `HttpClient` internals.
- Reports go to `reports/junit.xml` and `coverage/`; both are gitignored.
