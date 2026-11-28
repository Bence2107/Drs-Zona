---
paths:
  - "server/API/**"
  - "server/Services/**/AuthService.cs"
  - "server/Entities/Extensions/JwtSettings.cs"
  - "client/src/app/guards/**"
  - "client/src/app/interceptors/**"
  - "client/src/app/services/api/auth.service.ts"
  - "client/src/app/app.routes.ts"
---

# Authorization

- Roles are `User`, `Author`, `Editor` and `Admin`. Policies are defined once in `server/API/Program.cs`: `AdminOnly`, `EditorOrAdmin`, `AuthorOrAdmin`.
- Use `[Authorize(Policy = "...")]` on the action; plain `[Authorize]` means any signed-in user.
- Never remove or loosen an `[Authorize]` attribute or a policy unless asked. The API is the security boundary.
- Client guards (`authGuard`, `guestGuard`, `authorGuard`, `editorGuard`) are UX only. A guard is never a reason to drop a server check.
- A new role or policy needs both sides: a policy in `Program.cs` and a functional guard in `client/src/app/guards/`.
- Passwords are hashed with BCrypt and tokens signed with HS256 from `JWT_SECRET`. Do not change the algorithms, claims or token lifetime defaults.
- On the client, only `services/api/auth.service.ts` stores and reads the JWT; `authInterceptor` attaches it. Nothing else touches the token.
