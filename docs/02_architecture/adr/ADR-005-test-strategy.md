# ADR-005: Tesztstratégia – xUnit, háromszintű piramis, in-memory DB

- **Dátum:** 2026-02-01
- **Státusz:** Accepted

---

## Context

AI-asszisztált fejlesztésnél a tesztelés nem opcionális: az AI által generált kód
regressziós védelmet igényel. A projekt három réteget tartalmaz (Controller, Service,
Repository), mindegyik más szintű tesztet igényel. A teszteknek gyorsnak és
stabilnak kell lenniük, külső függőség (valódi DB, hálózat) nélkül is futtathatók.

Kényszerek:
- Lokális fejlesztői környezet, nincs CI pipeline (egyelőre)
- Minden service-hez legyen unit teszt, a kritikus flow-khoz integrációs teszt
- A tesztek izoláltak legyenek (teszt-közi állapot szivárgás nélkül)

---

## Decision

xUnit framework-öt alkalmazunk háromszintű tesztpiramisssal: unit tesztek mock
repository-val (Service réteg), integrációs tesztek EF Core in-memory DB-vel,
controller tesztek WebApplicationFactory-val; minden szint külön mappában.

---

## Alternatívák

**A) Csak unit tesztek**
- Gyors, izolált
- A DB interakciók és HTTP réteg nincs lefedve
- Hamis biztonságérzet: unit tesztek átmennek, de az integrált rendszer elromolhat

**B) Csak e2e tesztek (pl. Playwright)**
- Valódi felhasználói flow-kat tesztel
- Lassú, törékeny, nehéz hibát lokalizálni
- Frontend + backend együtt kell fusson

**C) NUnit vagy MSTest**
- Hasonló képességek
- xUnit a .NET ökoszisztémában de facto standard, jobban integrált az új tooling-gal
- `[Theory]` + `[InlineData]` paraméterezett tesztek tisztább szintaxisa

---

## Következmények

**Pozitív:**
- 396 teszt fut ~22 másodperc alatt lokálisan
- In-memory DB: minden integrációs teszt izolált, nincs teszt-közi szivárgás
- WebApplicationFactory: valódi HTTP stack tesztelése külső szerver nélkül
- Háromszintű piramis: gyors visszajelzés (unit) + konfidencia (integráció) + contract (controller)

**Negatív / kockázat:**
- EF Core in-memory provider néhány PostgreSQL-specifikus viselkedést nem szimulál (pl. unique constraint enforcement eltér)
- Nincs Angular e2e teszt – a frontend logika nincs automatizáltan lefedve
- CI pipeline hiányában a tesztek csak manuálisan futnak

---

## Verification

- `dotnet test` – 396/396 passed, 0 failed (lásd: `test_results.trx`)
- Futási idő: ~22 másodperc lokálisan hardvertől függően
- Minden service-hez van legalább 1 unit + 1 integrációs teszt
