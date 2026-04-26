# Quality Attributes

## Nemfunkcionális elvárások

| # | Attribútum            | Elvárás                                              | Mérőszám                                                        |
|---|-----------------------|------------------------------------------------------|-----------------------------------------------------------------|
| 1 | **Teljesítmény**      | API válaszidő normál terhelésen elfogadható          | p95 latency < 500ms lokális környezetben                        |
| 2 | **Biztonság**         | Jogosulatlan hozzáférés megakadályozása              | 100% védett endpoint 401-et ad vissza token nélkül              |
| 3 | **Megbízhatóság**     | Hibás input ne okozzon szerveromlást                 | 0 unhandled exception, minden hiba ResponseResult-ba csomagolva |
| 4 | **Karbantarthatóság** | Új feature hozzáadható a meglévő kód érintése nélkül | Réteges architektúra, service/repository szétválasztva          |
| 5 | **Tesztelhetőség**    | Az üzleti logika izoláltan tesztelhető               | 396 automata teszt, unit tesztek DB nélkül futnak               |
| 6 | **Konzisztencia**     | Minden API válasz egységes formátumú                 | 100% ResponseResult<T> wrapper használat                        |
| 7 | **Adatintegritás**    | Kapcsolódó adatok törléskor konzisztensek maradnak   | FK constraint + CASCADE/SET NULL szabályok DB szinten           |
| 8 | **Fejlesztői élmény** | API kliens automatikusan generálható a sémából       | ng-openapi-gen futtatható, típusbiztos Angular kliens           |

---

## Quality Attribute Scenario #1 – Biztonság

**Téma:** Védett endpoint elérési kísérlet érvénytelen tokennel

| Elem          | Leírás                                                                                                                                                    |
|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Forrás**    | Támadó vagy nem bejelentkezett felhasználó                                                                                                                |
| **Stimulus**  | HTTP kérés érkezik egy `[Authorize]` attribútummal védett endpointra, Authorization header nélkül vagy lejárt JWT tokennel                                |
| **Környezet** | Normál működési állapot, szerver elérhető                                                                                                                 |
| **Artefakt**  | ASP.NET Core auth middleware + Controller réteg                                                                                                           |
| **Válasz**    | A middleware elutasítja a kérést, `401 Unauthorized` HTTP választ ad vissza, a controller kód nem fut le; a válasz egységes hibaüzenetet tartalmaz        |
| **Mérőszám**  | 100% – minden védett endpoint 401-et ad érvénytelen token esetén; 69 controller teszt bizonyítja, köztük pl. `GetMe_ShouldReturn401_WhenNotAuthenticated` |

---

## Quality Attribute Scenario #2 – Megbízhatóság

**Téma:** Hibás / nem létező erőforrásra irányuló kérés

| Elem          | Leírás                                                                                                                                                                         |
|---------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Forrás**    | Felhasználó vagy frontend alkalmazás                                                                                                                                           |
| **Stimulus**  | GET/PUT/DELETE kérés olyan erőforrásra, amely nem létezik az adatbázisban (pl. törölt vezető ID-ja)                                                                            |
| **Környezet** | Normál működési állapot                                                                                                                                                        |
| **Artefakt**  | Service réteg + Repository réteg + ResponseResult                                                                                                                              |
| **Válasz**    | A service `ResponseResult.Failure("Nem található")` értékkel tér vissza; a controller `404 Not Found` választ küld; nincs unhandled exception, a szerver futása nem szakad meg |
| **Mérőszám**  | 0 unhandled exception not-found esetén; unit és integrációs tesztek tartalmazzák a not-found eseteket, mind passed (396/396)                                                   |

---

## Quality Attribute Scenario #3 – Adatintegritás

**Téma:** Felhasználó törlése kaszkád hatással

| Elem          | Leírás                                                                                                                                  |
|---------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| **Forrás**    | Admin felhasználó                                                                                                                       |
| **Stimulus**  | `DELETE /users/{id}` kérés egy aktív felhasználóra, akinek vannak kommentjei és szavazatai                                              |
| **Környezet** | Normál működés, PostgreSQL elérhető                                                                                                     |
| **Artefakt**  | PostgreSQL FK constraint-ek (`ON DELETE CASCADE` / `SET NULL`)                                                                          |
| **Válasz**    | A DB automatikusan törli a kapcsolódó kommenteket és szavazatokat; a cikkek megmaradnak (`SET NULL` author); árva rekord nem keletkezik |
| **Mérőszám**  | 0 árva rekord törlés után; integrációs teszt ellenőrzi (`Delete_ShouldSucceed_AndRemoveUserFromDb`)                                     |
