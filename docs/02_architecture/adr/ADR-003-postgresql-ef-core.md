# ADR-003: PostgreSQL adatbázis + Entity Framework Core ORM

- **Dátum:** 2026-02-01
- **Státusz:** Accepted

---

## Context

Az alkalmazásnak perzisztens adattárolásra van szüksége. Több entitás között komplex
kapcsolatok vannak (Driver ↔ Constructor ↔ Series ↔ GrandPrix ↔ Results), amelyek
relációs modellben természetesen ábrázolhatók. A migrációkezelés és a séma evolúció
fontos szempont a fejlesztés során.

Kényszerek:
- .NET 9 / ASP.NET Core backend
- Komplex JOIN-ok és aggregálások szükségesek (standings, results)
- Fejlesztői gépén lokálisan fut, nincs managed cloud DB igény
- Code-first megközelítés preferált (C# osztályokból séma)

---

## Decision

PostgreSQL-t használunk adatbázisként, Entity Framework Core ORM-mel,
code-first migrációkkal, az Npgsql provider biztosítja a kapcsolatot.

---

## Alternatívák

**A) SQLite**
- Zero-config, fájl alapú
- Konkurens írás korlátozott, production-ra nem alkalmas
- Komplex JOIN teljesítménye gyengébb nagy adatmennyiségnél

**B) SQL Server (LocalDB)**
- .NET-tel natív integráció
- Windows-függő (LocalDB), cross-platform fejlesztés nehézkes
- Licenc kötöttség production környezetben

**C) MongoDB (NoSQL)**
- Séma nélküli, flexibilis
- Relációs adatok (1:N, N:M kapcsolatok) JOIN nélkül nehézkesek
- EF Core támogatás limitált NoSQL esetén

---

## Következmények

**Pozitív:**
- Erős ACID garancia, komplex tranzakciók támogatása
- EF Core migrációk verziókövethetők, visszagörgethetők
- `UseInMemoryDatabase` tesztekben DB nélküli tesztelést tesz lehetővé
- UUID primary key natívan támogatott (`gen_random_uuid()`)

**Negatív / kockázat:**
- PostgreSQL service fut a fejlesztői gépen (extra dependency)
- EF Core LINQ → SQL fordítás nem mindig optimális komplex lekérdezéseknél
- In-memory provider viselkedése néhány edge case-ben eltér a valódi PostgreSQL-től

---

## Verification

- `dotnet ef migrations list` – migrációk listázhatók és visszagörgethetők
- 143 integrációs teszt fut EF Core in-memory DB-n, mind passed
- `dotnet ef dbcontext script` – teljes séma exportálható (`schema.sql`)
