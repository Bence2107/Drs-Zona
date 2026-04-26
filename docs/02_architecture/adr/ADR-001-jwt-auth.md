# ADR-001: JWT Bearer token alapú autentikáció

- **Dátum:** 2026-02-01
- **Státusz:** Accepted

---

## Context

A rendszernek autentikációt kell biztosítania a felhasználók számára. 
A backend ASP.NET Core API, a frontend Angular SPA. A két komponens külön fut, szerver oldali session kezelés ezért komplikált lenne. 
Az SPA architektúra stateless kommunikációt igényel.

Kényszerek:
- Frontend és backend teljesen szétválasztott (különböző portok)
- Nincs szükség külső identity provider integrációra
- Egyszerű role-alapú jogosultságkezelés kell (user / admin)

---

## Decision

JWT Bearer tokent alkalmazunk rövid lejárattal; a token tartalmazza a `userId` és `role` claim-eket, a backend minden kérésnél validálja.

---

## Alternatívák

**A) Session alapú auth (ASP.NET Core Identity + cookie)**
- Szerver oldali session tárolás szükséges
- SPA-val nehézkes (CORS + cookie kezelés)
- Skálázásnál sticky session vagy Redis kell

**B) OAuth2 / külső identity provider (Google, Auth0)**
- Erős biztonság, nem kell jelszót tárolni
- Fejlesztési overhead nagy, külső függőség keletkezik
- Lokális fejlesztési környezetben túlzott komplexitás

**C) JWT + Refresh token**
- Biztonságosabb (rövid access token + hosszú refresh token)
- Nagyobb implementációs komplexitás
- Jelenlegi scope-hoz túlzott

---

## Következmények

**Pozitív:**
- Stateless – szerver nem tárol session állapotot
- SPA-val egyszerűen integrálható (Authorization header)
- Könnyen tesztelhető (token generálás tesztben)

**Negatív / kockázat:**
- Token lejárat előtt nem visszavonható (csak `is_logged_in` DB flag-gel védhető)
- Token localStorage-ban tárolva XSS esetén veszélyes
- Refresh token hiánya miatt a felhasználó lejárat után kijelentkezik

---

## Verification

- 69 controller teszt ellenőrzi, hogy minden védett endpoint 401-et ad érvénytelen token esetén
- `Login_ShouldReturn200_WithToken` teszt ellenőrzi a token generálást
- `is_logged_in` flag DB-ben: `Logout_ShouldSucceed_AndSetIsLoggedInFalse` integrációs teszt
