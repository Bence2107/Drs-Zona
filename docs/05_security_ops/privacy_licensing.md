# Privacy + Adatkezelés + Licencek

---

## 1. Adatkategóriák

| Kategória                   | Mezők                                                             | PII?                | Megjegyzés                                   |
|-----------------------------|-------------------------------------------------------------------|---------------------|----------------------------------------------|
| **Felhasználói azonosítók** | `id` (uuid), `username`, `full_name`, `email`                     | ✅ Igen             | Regisztrációkor megadott adatok              |
| **Hitelesítési adat**       | `password_hash`                                                   | ✅ Igen (érzékeny)  | Bcrypt hash, plain text soha nem tárolódik   |
| **Session adat**            | `is_logged_in`, `current_session_id`, `last_login`, `last_active` | ⚠️ Részben           | Technikai adat, de felhasználóhoz köthető    |
| **Tartalmi adat**           | `articles`, `comments`, `polls`, `poll_votes`                     | ⚠️ Közvetve          | Pl. `author_id` révén felhasználóhoz köthető |
| **Verseny adat**            | `drivers`, `constructors`, `results`, `grands_prix`               | ❌ Nem              | Nyilvános sportadat, nem személyes           |
| **Aktivitási adat**         | `comment_votes`, `poll_votes`                                     | ⚠️ Közvetve          | Szavazat felhasználóhoz köthető              |

---

## 2. Adatáramlás

```
Felhasználó (böngésző)
    │
    ▼
Angular SPA (frontend)
    │  HTTP/JSON (JWT Bearer token)
    ▼
ASP.NET Core API (backend)
    │  EF Core
    ▼
PostgreSQL (lokális DB)
```

- **Külső szolgáltatásba nem kerül adat** – nincs analytics, nincs CDN, nincs AI integráció
- **Email küldés nincs** – nincs SMTP, email csak tárolva van, nem küldve sehova
- **Avatar tárolás:** `has_avatar` boolean flag – a tényleges fájl tárolási helye a backend `api/uploads` mappájában.

---

## 3. Adatmegőrzés és törlés

| Adat                      | Megőrzés                                      | Törlési mechanizmus                                               |
|---------------------------|-----------------------------------------------|-------------------------------------------------------------------|
| Felhasználói fiók         | A fiók törléséig                              | `DELETE /users/{id}` – `ON DELETE CASCADE` a kapcsolódó táblákban |
| Kommentek                 | Fiók törlésekor törlődik                      | `FK_comments_users ON DELETE CASCADE`                             |
| Szavazatok (`poll_votes`) | Fiók törlésekor `SET NULL`                    | `FK_poll_votes_users ON DELETE SET NULL`                          |
| Cikkek                    | Fiók törlésekor `SET NULL` (cikk megmarad)    | `FK_articles_users ON DELETE SET NULL`                            |
| Session adat              | Logout után nullázva                          | `AuthService.Logout` törli a `current_session_id`-t               |
| Logok                     | Fejlesztői környezetben nincs perzisztált log | –                                                                 |

---

## 4. Hozzáférés és szerepkörök

| Szerepkör                       | Mit lát / módosíthat                                                      |
|---------------------------------|---------------------------------------------------------------------------|
| **Vendég (nem bejelentkezett)** | Publikus cikkek, futameredmények, bajnokság állás, aktív szavazások       |
| **User**                        | Saját profil, kommentek írása/szerkesztése, szavazás                      |
| **Writer**                      | Saját hír írása, managelése                                               |
| **Editor**                      | Bajnokságok, pilóták, konstruktőrök, szerződések, eredmények managelése   | 
| **Admin**                       | Minden adat kezelése, beleértve a nem saját híreket is                    |

Admin funkciók:
- Eredmények módosítása, újraszámítás
- Bajnokság státusz frissítése
- Összes hír módosítása, törlése
- Minden CRUD művelet

---

## 5. AI használat és adatok

**A rendszer nem küld adatot semmilyen AI szolgáltatónak.**

- Nincs OpenAI / Claude / egyéb LLM integráció
- PII nem kerül külső rendszerbe
- A fejlesztés során AI (Claude, Google Gemini) segítette a kódírást, de production adatot nem kapott

---

## 6. Harmadik fél függőségek és licencek

### Backend (.NET / NuGet)

| Csomag                                        | Verzió  | Licensz                              | Felhasználás                                                    |
|-----------------------------------------------|---------|--------------------------------------|-----------------------------------------------------------------|
| DotNetEnv                                     | 3.1.1   | MIT                                  | `.env` fájlból környezeti változók betöltése                    |
| Microsoft.AspNetCore.OpenApi                  | 9.0.12  | MIT                                  | OpenAPI/Swagger dokumentáció generálása az ASP.NET Core API-hoz |
| Microsoft.EntityFrameworkCore                 | 9.0.12  | MIT                                  | ORM alap – adatbázis-entitások kezelése                         |
| Microsoft.EntityFrameworkCore.Relational      | 9.0.12  | MIT                                  | Relációs adatbázis-specifikus EF Core funkciók                  |
| Microsoft.EntityFrameworkCore.Design          | 9.0.12  | MIT                                  | Design-time eszközök (migráció generáláshoz szükséges)          |
| Microsoft.EntityFrameworkCore.Tools           | 9.0.12  | MIT                                  | CLI eszközök EF Core migrációk futtatásához                     |
| Microsoft.EntityFrameworkCore.InMemory        | 9.0.12  | MIT                                  | In-memory adatbázis tesztelési célokra                          |
| Npgsql.EntityFrameworkCore.PostgreSQL         | 9.0.4   | PostgreSQL (MIT-szerű)               | EF Core PostgreSQL provider                                     |
| Swashbuckle.AspNetCore                        | 9.0.6   | MIT                                  | Swagger UI és OpenAPI specifikáció generálása                   |
| BCrypt.Net-Next                               | 4.0.3   | MIT                                  | Jelszavak bcrypt algoritmussal való hashelése                   |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.12  | MIT                                  | JWT token alapú hitelesítés kezelése                            |
| SixLabors.ImageSharp                          | 3.1.12  | Six Labors Split License (MIT szerű) | Képfeldolgozás (átméretezés, formátumkonverzió stb.)            |
| System.IdentityModel.Tokens.Jwt               | 8.15.0  | MIT                                  | JWT tokenek létrehozása és validálása                           |
| coverlet.collector                            | 8.0.1   | MIT                                  | Kódlefedettség-mérés tesztek futtatásakor                       |
| coverlet.msbuild                              | 8.0.1   | MIT                                  | Kódlefedettség-mérés MSBuild integráción keresztül              |
| FluentAssertions                              | 7.0.0   | Apache 2.0                           | Olvasható, fluent stílusú unit teszt assertek                   |
| JunitXml.TestLogger                           | 5.0.0   | MIT                                  | JUnit XML formátumú teszteredmény-riport (CI/CD)                |
| Microsoft.AspNetCore.Mvc.Testing              | 9.0.12  | MIT                                  | Integrációs tesztelés ASP.NET Core appokhoz                     |
| Microsoft.NET.Test.Sdk                        | 17.12.0 | MIT                                  | .NET tesztfuttató keretrendszer alap                            |
| Moq                                           | 4.20.72 | BSD-3-Clause                         | Mock objektumok létrehozása unit tesztekhez                     |
| xunit                                         | 2.9.2   | Apache 2.0                           | Unit teszt keretrendszer                                        |
| xunit.runner.visualstudio                     | 2.8.2   | Apache 2.0                           | xUnit tesztek futtatása Visual Studio-ban / dotnet test-tel     |

### Frontend (npm / Angular)

| Csomag                            | Verzió   | Licensz      |  Felhasználás                                         |
|-----------------------------------|----------|--------------|-------------------------------------------------------|
| @angular/animations               | 21.2.2   | MIT          | Angular animációs modul                               |
| @angular/build                    | 21.2.1   | MIT          | Angular build eszközök (esbuild alapú)                |
| @angular/cdk                      | 21.2.1   | MIT          | Angular Component Dev Kit – UI komponens alapok       |
| @angular/cli                      | 21.2.1   | MIT          | Angular parancssori eszköz (ng parancsok)             |
| @angular/common                   | 21.2.2   | MIT          | Általános Angular szolgáltatások és pipe-ok           |
| @angular/compiler                 | 21.2.2   | MIT          | Angular sablon-fordító                                |
| @angular/compiler-cli             | 21.2.2   | MIT          | AOT fordításhoz szükséges CLI eszköz                  |
| @angular/core                     | 21.2.2   | MIT          | Angular mag – komponensek, DI, lifecycle              |
| @angular/forms                    | 21.2.2   | MIT          | Reaktív és template-alapú űrlapok kezelése            |
| @angular/material                 | 21.2.1   | MIT          | Material Design UI komponenskönyvtár                  |
| @angular/platform-browser         | 21.2.2   | MIT          | Böngészős futtatókörnyezet                            |
| @angular/platform-browser-dynamic | 21.2.2   | MIT          | JIT fordítással való böngészős indítás                |
| @angular/router                   | 21.2.2   | MIT          | Oldalak közötti navigáció és útvonalkezelés           |
| @kolkov/angular-editor            | 3.0.5    | MIT          | WYSIWYG / Rich Text szerkesztő Angular-hoz            |
| @types/jasmine                    | 5.1.15   | MIT          | TypeScript típusdefiníciók a Jasmine keretrendszerhez |
| @types/uuid                       | 10.0.0   | MIT          | TypeScript típusdefiníciók az uuid csomaghoz          |
| jasmine-core                      | 5.6.0    | MIT          | Jasmine unit teszt keretrendszer magkönyvtára         |
| karma                             | 6.4.4    | MIT          | Böngésző-alapú tesztfuttató                           |
| karma-chrome-launcher             | 3.2.0    | MIT          | Chrome böngésző indítása Karma alatt                  |
| karma-coverage                    | 2.2.1    | MIT          | Kódlefedettség-mérés Karmával                         |
| karma-jasmine                     | 5.1.0    | MIT          | Jasmine adapter Karma tesztfuttatóhoz                 |
| karma-jasmine-html-reporter       | 2.1.0    | MIT          | HTML riport generálása Jasmine tesztekhez             |
| karma-junit-reporter              | 2.0.1    | MIT          | JUnit XML formátumú tesztriport (CI/CD)               |
| ng-openapi-gen                    | 1.0.5    | MIT          | OpenAPI 3.x specifikációból Angular kód generálása    |
| rxjs                              | 7.8.2    | Apache 2.0   | Reaktív programozás – Observable alapú adatfolyamok   |
| tslib                             | 2.8.1    | BSD-0-Clause | TypeScript helper függvények futásidejű könyvtára     |
| typescript                        | 5.9.3    | Apache 2.0   | TypeScript fordító és nyelvi eszközök                 |
| uuid                              | 13.0.0   | MIT          | UUID generálás                                        |
| zone.js                           | 0.15.1   | MIT          | Angular változásdetektáláshoz szükséges zóna-könyvtár |


### Licenc kompatibilitás

Minden használt függőség **MIT vagy Apache 2.0** licencű, amelyek:
- Kereskedelmi és oktatási célra szabadon használhatók
- Nem kötelezik a forráskód nyilvánosságra hozatalára
- Kompatibilisek egymással

**Nincs GPL vagy egyéb copyleft licencű függőség** a közvetlen függőségek között.
