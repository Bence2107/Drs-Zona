# Verification Log

> Az AI által javasolt megoldások ellenőrzésének naplója.

---

## V-01 – JWT token visszavonhatatlansága

- **AI állítás / javaslat:** JWT Bearer token elegendő autentikációhoz; a token lejáratig érvényes
- **Kockázat:** Ha a felhasználó kijelentkezik, a token még lejáratig használható – session hijacking lehetséges
- **Ellenőrzés módja:** Code review + integrációs teszt: logout után a `is_logged_in` flag ellenőrzése DB-ben
- **Eredmény:** PASS – `Logout_ShouldSucceed_AndSetIsLoggedInFalse` teszt konfirmálja; a backend minden kérésnél ellenőrzi a `is_logged_in` státuszt
- **Következtetés:** A `current_session_id` és `is_logged_in` mezők hozzáadva a `users` táblához, logout után a szerver oldal érvényteleníti a tokent

---

## V-02 – EF Core in-memory vs. PostgreSQL viselkedés

- **AI állítás / javaslat:** Az EF Core in-memory provider megfelelő integrációs teszteléshez
- **Kockázat:** Az in-memory provider nem enforcolja a UNIQUE constraint-eket és FK szabályokat ugyanúgy mint PostgreSQL – hamis pozitív tesztek
- **Ellenőrzés módja:** Manuális teszt: duplikált email regisztrációt próbáltam in-memory DB-vel
- **Eredmény:** RÉSZLEGES PASS – néhány constraint (pl. UNIQUE index) eltérően viselkedik; a service réteg üzleti logikával kompenzálja (duplikáció ellenőrzés kódban)
- **Következtetés:** A service rétegben explicit duplikáció-ellenőrzés van (`Register_ShouldFail_WhenEmailAlreadyTaken`), nem csak DB constraint-re hagyatkozva; dokumentálva az ismert hiányosságok között

---

## V-03 – NuGet verzióhiba – AI javaslat elégtelensége

- **AI állítás / javaslat:** A `dotnet build` verziówarningokat a `.csproj` PackageReference verziók manuális felülírásával kell javítani
- **Kockázat:** Rossz irányú debug, időveszteség; a valódi hiba rejtve marad
- **Ellenőrzés módja:** Az AI javaslat alkalmazása után a warning megmaradt; saját debug: `dotnet nuget locals all --clear` + cache törlés + újratelepítés
- **Eredmény:** FAIL (AI javaslat) → PASS (saját megoldás) – a valódi ok automatikusan rosszul letöltött csomag volt, nem verziókonfiguráció
- **Következtetés:** Az AI hallucináció-szerű választ adott ismeretlen kontextusban, dokumentálva hogy build hibáknál az AI javaslat csak kiindulópont, nem végső megoldás

---

## V-04 – Generált tesztek lefedettség ellenőrzése

- **AI állítás / javaslat:** A generált xUnit tesztek lefedik az összes kritikus service metódust
- **Kockázat:** Hiányzó edge case-ek (pl. null input, üres lista, boundary értékek) – regresszió védelme gyengébb
- **Ellenőrzés módja:** Tesztek manuális átnézése service-enként; hiányzó esetek azonosítása és pótlása
- **Eredmény:** PASS az alapeseteknél, RÉSZLEGES FAIL edge case-eknél – például `GetRecentArticles_ShouldReturnEmptyList_WhenNoArticles` és `CreateParticipations_ShouldSkip_WhenAlreadyExists` kézzel pótolva
- **Következtetés:** 396 teszt végeredmény, kézzel pótolt edge case-ekkel, az AI által generált tesztek jó alapot adtak de nem voltak teljesek

---

## V-05 – DTO validációs attribútumok helyessége

- **AI állítás / javaslat:** A generált DTO-k `[Required]`, `[MaxLength]`, `[Range]` attribútumai megfelelőek
- **Kockázat:** Hibás validációs szabályok átengednék az érvénytelen adatokat, vagy blokkolnák az érvényeseket
- **Ellenőrzés módja:** Minden DTO attribútum összevetése az adatbázis séma constraint-jeivel (`schema.sql`); pl. `username varchar(50)` → `[MaxLength(50)]`
- **Eredmény:** RÉSZLEGES PASS – az alapesetek helyesek voltak; néhány `[Range]` és opcionális mező hibásan volt jelölve
- **Következtetés:** DTO-k módosítva: felesleges `[Required]` attribútumok eltávolítva nullable mezőkről, `[Range]` értékek a DB típusokhoz igazítva

---

## V-06 – SQL seed adatok UUID és timestamp formátuma

- **AI állítás / javaslat:** A generált SQL INSERT szkriptek helyesen tartalmazzák az UUID és timestamptz értékeket
- **Kockázat:** Hibás formátum esetén az adatbevitel meghiúsul vagy silent data corruption keletkezik
- **Ellenőrzés módja:** `psql` parancssorban a seed szkriptek futtatása, PostgreSQL hibaüzenetek ellenőrzése
- **Eredmény:** RÉSZLEGES FAIL – több helyen UUID idézőjel formátum hiba (`{uuid}` vs. `'uuid'`), timezone suffix hiánya (`2024-01-01` vs. `2024-01-01T00:00:00+00`)
- **Következtetés:** Seed szkriptek kézzel javítva; a PostgreSQL-specifikus formátumokat az AI nem kezelte konzisztensen

---

## V-07 – Record vs. class döntés – EF Core kompatibilitás

- **AI állítás / javaslat:** C# record típus javasolt DTO-khoz, mert immutable és value equality szemantikája van
- **Kockázat:** EF Core bizonyos verziókban problémásabb a record típusú entitásokkal (change tracking, proxy generálás)
- **Ellenőrzés módja:** Microsoft EF Core dokumentáció átnézése; PoC: record típusú DTO kipróbálása EF Core context-tel
- **Eredmény:** FAIL: egyik kontextusban a recordot, másiknál a class-ot hozta fel jobb opciónak
- **Következtetés:** DTO-khoz recordot használtam, mert egyszerűbb volt.

---

## V-08 – Async refaktor helyessége

- **AI állítás / javaslat:** Az összes service metódus biztonságosan átírható async/await-re
- **Kockázat:** Helytelenül async-é tett szinkron metódusok deadlock-ot okozhatnak, vagy felesleges overhead-et adnak
- **Ellenőrzés módja:** Code review: minden `async` metódus tartalmaz-e legalább egy `await` hívást; `dotnet build` warning-ok ellenőrzése
- **Eredmény:** RÉSZLEGES FAIL – néhány metódus `async` kulcsszót kapott `await` nélkül (compiler warning: CS1998); ezek visszaállítva szinkronra
- **Következtetés:** Az async refaktor után `dotnet build` 0 warning állapotba hozva; az érintett metódusok azonosítva és javítva

---

## V-09 – WYSIWYG könyvtár licenc ellenőrzés

- **AI állítás / javaslat:** A `kolkov/angular-editor` megfelelő Angular rich text editorhoz
- **Kockázat:** Nem kompatibilis licenc (pl. GPL) kötelezővé tehetné a forráskód nyilvánosságra hozatalát
- **Ellenőrzés módja:** GitHub Repo lincensz ellenőrzése, más implementációk keresése.
- **Eredmény:** PASS – a csomag MIT licencű, kereskedelmi és oktatási célra szabadon használható
- **Következtetés:** Könyvtár integrálva; licenc dokumentálva [`privacy_licensing.md`](../05_security_ops/privacy_licensing.md)-ben

---

## V-10 – Loading container szélesség hiba – AI korlátja

- **AI állítás / javaslat:** A loading container szélességi CSS hibát meg tudja oldani
- **Kockázat:** Hibás CSS fix rosszabb UX-et okozhat, vagy csak részlegesen javítja a problémát
- **Ellenőrzés módja:** Az AI több javaslatot adott (`width: 100%`, `flex: 1`, `max-width` módosítások) – egyik sem oldotta meg a problémát konzisztensen
- **Eredmény:** FAIL – az AI nem tudta megoldani kellő vizuális kontextus nélkül; a hiba ismert, alacsony prioritású
- **Következtetés:** Dokumentálva mint ismert UI hiányosság; az AI vizuális/layout hibáknál kevésbé megbízható szöveges leírás alapján
