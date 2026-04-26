# Threat Model

## Áttekintés

A rendszer jelenleg lokális fejlesztői környezetben fut, publikus interneten a https://drs-zona.hu/ címen lesz elérhető.
A threat model a tervezett vagy jövőbeli deployment esetére is érvényes megállapításokat tartalmaz.

---

## 1. Attack surface (belépési pontok)

| Belépési pon t       | Leírás                                                                     |
|----------------------|----------------------------------------------------------------------------|
| **REST API**         | ASP.NET Core backend, `/api/v1/*` endpointok. JWT alapú auth.              |
| **Angular frontend** | SPA, böngészőből érhető el. Form inputok, HTTP kérések.                    |
| **Swagger UI**       | `/swagger/index.html` – fejlesztői eszköz, endpointokat listáz és hívható. |
| **PostgreSQL**       | Csak lokálisan elérhető, közvetlenül nem exponált.                         |
| **Admin endpointok** | Role-alapú védett endpointok (`role = 'admin'`).                           |

---

## 2. Fenyegetések (STRIDE)

| # | Kategória                   | Fenyegetés                                                        | Hatás                                     | Valószínűség                           |
|---|-----------------------------|-------------------------------------------------------------------|-------------------------------------------|----------------------------------------|
| T1 | **Spoofing**               | JWT token ellopása (XSS vagy insecure storage)                    | Más felhasználó nevében végzett műveletek | Közepes                                |
| T2 | **Tampering**              | Kérés módosítása (pl. `userId` vagy `role` mező manipulálása)     | Jogosulatlan adatmódosítás                | Alacsony (JWT validálva)               |
| T3 | **Repudiation**            | Nincs audit log – tagadható, hogy ki hajtott végre műveletet      | Felelősség nem bizonyítható               | Közepes                                |
| T4 | **Information Disclosure** | Részletes hibaüzenet / stack trace kiszivárgása                   | Belső struktúra felfedése támadónak       | Alacsony (ErrorResponse szűr)          |
| T5 | **Information Disclosure** | PII (email, jelszóhash) bekerül logba                             | Adatszivárgás, privacy sértés             | Alacsony (logging policy van)          |
| T6 | **Denial of Service**      | Nincs rate limiting – API flood lehetséges                        | Szolgáltatás lelassulása / leállása       | Közepes (lokál, de kockázat)           |
| T7 | **Elevation of Privilege** | Admin endpoint elérhető normál JWT tokennel, ha role check hibás  | Jogosulatlan admin műveletek              | Alacsony (role check implementálva)    |
| T8 | **Tampering**              | SQL injection ORM megkerülésével (raw query)                      | Adatbázis manipuláció                     | Alacsony (EF Core parameterized query) |

---

## 3. Mitigációk

### T1 – JWT token ellopás
- **Mitigáció:** JWT rövid lejárati idő; `is_logged_in` flag DB-ben, logout után token érvénytelen
- **Kód:** `AuthService.Logout` nullázza a session state-et
- **Hiány:** HttpOnly cookie helyett Authorization header – frontend XSS esetén kockázat

### T2 – Kérés manipulálás
- **Mitigáció:** A JWT payload-ból kerül ki a `userId`, nem a request bodyból
- **Kód:** Controller-ek `[Authorize]` attribútummal védve, claim-ből olvasva az identity

### T3 – Repudiation (audit log hiánya)
- **Mitigáció:** Jelenleg nincs audit log – ez ismert residual risk
- **Jövőbeli:** Middleware-szintű request logging bevezetése tervezett

### T4 – Stack trace szivárgás
- **Mitigáció:** `ResponseResult<T>` egységes hibamodell, belső részletek nem kerülnek ki
- **Kód:** `error_handling.md` szerint a `500`-as hibák csak generikus üzenetet adnak

### T5 – PII a logban
- **Mitigáció:** Logging policy tiltja jelszó, token, email logolását
- **Ellenőrzés:** Code review során ellenőrzött

### T6 – Rate limiting hiánya
- **Mitigáció:** Jelenleg nincs – lokális környezetben elfogadott kockázat
- **Jövőbeli:** ASP.NET Core `RateLimiter` middleware bevezetése tervezett publikus deploy előtt

### T7 – Privilege escalation
- **Mitigáció:** `[Authorize(Roles = "admin")]` attribútum minden admin endpointon
- **Teszt:** Controller tesztek ellenőrzik a 401-es visszatérést nem admin tokennél

### T8 – SQL injection
- **Mitigáció:** Kizárólag EF Core LINQ lekérdezések, raw SQL nincs használatban
- **Kód:** Repository réteg teljes mértékben ORM alapú

---

## 4. Residual risk (maradék kockázat)

| Kockázat                    | Miért fogadott el                                                            |
|-----------------------------|------------------------------------------------------------------------------|
| Nincs rate limiting         | Lokális környezetben nem releváns; publikus deploy előtt kezelendő           |
| Nincs audit log             | Fejlesztői időkeret korlát; nem kritikus a jelenlegi scope-ban               |
| JWT HttpOnly cookie hiánya  | SPA architektúra korlátja; XSS védelem más szinten (CSP, input sanitization) |
| Swagger UI nincs auth-védve | Lokális dev tool; production deployban letiltandó                            |

---

## 5. Verification

| Ellenőrzés                    | Módszer                                 | Státusz                         |
|-------------------------------|-----------------------------------------|---------------------------------|
| Auth endpoint védettség       | Controller tesztek (401 visszatérés)    | ✅ 396 teszt fut                |
| Role-alapú védelem            | `[Authorize(Roles)]` + controller teszt | ✅ Tesztelve                    |
| PII a logban                  | Code review                             | ✅ Manuálisan ellenőrzött       |
| Dependency vulnerability scan | `dotnet list package --vulnerable`      | ✅ Futtatva (0 hiba)            |
| SQL injection                 | EF Core LINQ only, nincs raw query      | ✅ Architektúra szintű védelem  |
