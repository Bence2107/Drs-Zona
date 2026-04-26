# Teszt stratégia

## 1. Teszt piramis

```
         /‾‾‾‾‾‾‾‾‾‾‾‾‾‾\
        /   Controller   \   69 teszt (17%)
       /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\
      /     Integration    \  143 teszt (36%)
     /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\
    /          Unit          \ 184 teszt (46%)
   /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\
```

| Szint                                   | Darab   | Arány    |
|-----------------------------------------|---------|----------|
| Unit (Services.Units)                   | 184     | 46%      |
| Integration (Services.Integrations)     | 143     | 36%      |
| Controller / API contract (Controllers) | 69      | 17%      |
| **Összesen**                            | **396** | **100%** |

---

## 2. Mit tesztel melyik szint

### Unit tesztek (`Tests.Services.Units`)
Tisztán service logikát tesztelnek, adatbázis nélkül. A repository réteg mock/in-memory
implementációval van helyettesítve.

Lefedett service-ek:
- `ArticleService` – cikk CRUD, summary szekciók kezelése
- `AuthService` – regisztráció, login, logout, jelszóváltás
- `ChampionshipService` – bajnokság létrehozás, státusz frissítés, participációk
- `CommentService` – komment CRUD, reply lánc, szavazás
- `ConstructorsService` – konstruktőr CRUD, brand kapcsolat
- `ContractsService` – vezető–konstruktőr szerződések
- `DriverService` – vezető CRUD, életkor validáció
- `GrandPrixService` – futam CRUD, session kezelés
- `PollService` – szavazás logika, lejárat, duplikáció védelem
- `ResultsService` – eredmény beszúrás, újraszámítás
- `SeriesService` – sorozat lekérés, szezon aggregálás
- `StandingsService` – pontszám összesítés, rangsorolás

Tipikus tesztelt esetek:
- Sikeres műveletek (happy path)
- Not found hibák (entitás nem létezik)
- Validációs hibák (pl. 15 évnél fiatalabb vezető)
- Edge case-ek (pl. üres lista visszaadás, duplikáció kihagyás)

### Integrációs tesztek (`Tests.Services.Integrations`)
Service + valódi EF Core in-memory adatbázis együtt tesztelve. Minden teszt friss
adatbázis-példányon fut (izolált). Ellenőrzik, hogy az adatok ténylegesen perzisztálódnak
és a lekérdezések helyesen működnek.

Lefedett területek:
- Adatperzisztencia: minden Create után ellenőrzés DB-ből olvasva
- Kaszkád törlés: pl. komment törlésekor a reply-ok is törlődnek
- Egyedi constraint-ek: duplikált regisztráció megakadályozása
- Pontszám újraszámítás: F1 pontozási rendszer validálása
- Auth flow: login/logout állapot DB-ben való megjelenése
- Jelszókezelés: hash tárolás ellenőrzése (nem plain text)

### Controller tesztek (`Tests.Controllers`)
HTTP szintű API contract tesztek `WebApplicationFactory` segítségével. Valódi HTTP
kérések mennek a teljes middleware stack-en keresztül.

Lefedett területek:
- HTTP státuszkódok helyessége (200, 400, 401, 404)
- Autentikáció és jogosultság ellenőrzés (védett endpointok 401-et adnak vissza)
- Admin vs. user szerepkör különbség
- Request/response contract stabilitása

---

## 3. Legkritikusabb user flow-k

### 1. Autentikáció flow
Regisztráció → Login → JWT token → Védett endpoint elérése → Logout

Lefedő tesztek:
- `Register_ShouldSucceed_AndPersistUser`
- `Login_ShouldSucceed_WithCorrectCredentials`
- `Login_ShouldSetIsLoggedInTrue_AfterSuccess`
- `Logout_ShouldSucceed_AndSetIsLoggedInFalse`
- `GetMe_ShouldReturn200_WhenAuthenticated`
- `GetMe_ShouldReturn401_WhenNotAuthenticated`

### 2. Versenyeredmény flow
Futam létrehozás → Eredmények rögzítése → Pontszám újraszámítás → Bajnokság állás lekérés

Lefedő tesztek:
- `Create_ShouldSucceed_AndPersistGrandPrix`
- `RecalculateSession_ShouldSucceed_AndRecalculateF1Points`
- `GetConstructorStandings_ShouldSumPointsPerConstructor_AndOrderDescending`
- `GetDriverStandings_ShouldGroupByDriver_AndSumPoints`

---

## 4. Mock/stub stratégia

### Unit teszteknél
- A repository interfészek in-memory fake implementációval vannak helyettesítve
- Nincs valódi DB kapcsolat → gyors futás, izolált logika tesztelés
- Minden teszt saját mock állapottal indul

### Integrációs teszteknél
- EF Core `UseInMemoryDatabase` provider
- Minden tesztnél új, egyedi DB példány (`Guid.NewGuid().ToString()` névvel)
- Nincs teszt-közi állapot szivárgás

### Controller teszteknél
- `WebApplicationFactory<Program>` a teljes ASP.NET Core pipeline-nal
- JWT token generálás tesztben, valódi auth middleware fut

---

## 5. Quality gate-ek

CI pipeline jelenleg nincs konfigurálva, de a tesztek lokálisan futtathatók és
dokumentált módon reprodukálhatók.

Tervezett quality gate-ek (CI bevezetésekor):
- `dotnet build` – fordítási hibák blokkolják a merge-t
- `dotnet test` – bármely failing teszt blokkolja a merge-t
- `dotnet format --verify-no-changes` – kódformázás ellenőrzés

---

## 6. Tesztek futtatása

### Összes teszt futtatása
```bash
cd server
dotnet test
```

### TRX riport generálása (dokumentációhoz)
```bash
cd server
dotnet test --logger "trx;LogFileName=test_results.trx" --results-directory ./docs/04_quality/
```

### Coverage gyűjtése
```bash
cd server
dotnet test --collect:"XPlat Code Coverage" --results-directory ./docs/04_quality/
```

### Csak unit tesztek
```bash
cd server
dotnet test --filter "FullyQualifiedName~Tests.Services.Units"
```

### Csak integrációs tesztek
```bash
cd server
dotnet test --filter "FullyQualifiedName~Tests.Services.Integrations"
```

### Csak controller tesztek
```bash
cd server
dotnet test --filter "FullyQualifiedName~Tests.Controllers"
```
