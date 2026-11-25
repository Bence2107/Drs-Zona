---
paths:
  - "server/Tests/**"
---

# Server tests

xUnit, Moq and FluentAssertions, on the EF Core in-memory provider; no database needed.

| Change | Test |
| --- | --- |
| Service logic | Unit test in `Tests/Services/Units/<Service>UnitTests.cs` with mocked repositories |
| Service + repository behaviour | Integration test in `Tests/Services/Integrations/` using `InMemoryDbFactory.CreateContext()` |
| Routes, auth, status codes | `Tests/Controllers/ControllerIntegrationTests.cs` through `TestWebAppFactory` (environment `Testing`) |

- Add tests to the existing file of that service; do not create a parallel structure.
- FluentAssertions stays on 7.x: version 8 changed to a commercial licence.
- `JWT_SECRET` must be set to run the controller tests (CI sets a dummy value).
- Run one suite with `dotnet test --filter "FullyQualifiedName~Tests.Services.Units"`.
