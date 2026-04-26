# Scope Contract — Drs-Zóna

## 🎯 Célkitűzés
Ez a dokumentum a Drs-Zóna projekt pontos hatókörét definiálja értékelési célból.
Meghatározza az MVP-ben szállítandó funkciókat, az opcionális fejlesztési célokat, és az explicit módon hatókörön kívüli elemeket.

---

# 🧩 MVP Felhasználói Sztorik

## US-01 — Hír létrehozása (Vivi, újságíró)
**Leírás:**
Vivi, az újságíró szeretne egy új hírcikket közzétenni a platformon, hogy a motorsport rajongók értesüljenek az aktuális eseményekről.

**Elfogadási kritériumok:**
- **Given** Vivi be van jelentkezve Writer jogosultságú felhasználóként
- **When** Megnyitja az „Hírek oldalt és rányom a + gomb-ra, aminek a MatToolTip-je "Új hír létrehozása"
- **Then** Kitölti a kötelező mezőket (cím, tartalom, kategória, borítókép), és a „Közzététel" gombra kattint
- **Then** A hír megjelenik a főoldalon és a megfelelő kategória alatt, az összes felhasználó láthatja
- **Then** A rendszer visszajelzést ad a sikeres közzétételről

---

## US-02 — Hír olvasása és kommentelése (Bence, olvasó)
**Leírás:**
Bence, a motorsport rajongó el szeretné olvasni a legfrissebb híreket, és véleményét hozzá szeretné fűzni egy cikkhez.

**Elfogadási kritériumok:**
- **Given** Bence be van jelentkezve regisztrált felhasználóként
- **When** Megnyit egy hírcikket, elolvassa, majd beír egy kommentet és elküldi
- **Then** A komment megjelenik a cikk alatt minden látogató számára, a szerző nevével és időbélyeggel ellátva
- **Then** A hír szerzője értesítést kap az új kommentről

---

## US-03 — Szavazáson való részvétel (Bence, olvasó)
**Leírás:**
Bence aktívan szeretne részt venni a közösség életében: szavazni a közösségi közvélemény-kutatásokon, hogy kiderüljön mások véleménye is.

**Elfogadási kritériumok:**
- **Given** Bence be van jelentkezve és megnyit egy szavazást tartalmazó oldalt
- **When** Kiválaszt egy szavazás kártyát, és rákattint
- **Then** Kiválasztja a szavazatát
- **Then** A szavazata rögzítésre kerül; egy felhasználó csak egyszer szavazhat ugyanarra a szavazásra

---

## US-04 — Versenyeredmények megtekintése (Bence, olvasó)
**Leírás:**
Bence szeretné nyomon követni a motorsport bajnokságok aktuális állását és az egyes futamok eredményeit sportáganként szűrve.

**Elfogadási kritériumok:**
- **Given** Bence megnyitja az „Eredmények" oldalt (bejelentkezés nélkül is elérhető)
- **When** Kiválaszt egy bajnokságot (pl. F1, WRC, MotoGP) és egy adott Versenyt.
- **Then** Megjelenik az adott verseny eredménye az adatbázisból.

---

## US-05 — Eredmény és bajnokság kezelése (Attila, szerkesztő)
**Leírás:**
Egy Editor jogosultságú szerkesztő szeretné felvinni, módosítani és kezelni a bajnokságok, versenyzők, konstruktőrök és szerződések adatait a rendszerben.

**Elfogadási kritériumok:**
- **Given** A felhasználó Editor jogosultsággal van bejelentkezve
- **When** Létrehoz vagy szerkeszt egy bajnokságot, versenyzőt, konstruktőrt vagy szerződést, majd menti a változtatásokat
- **Then** Az adatbázis frissül, a változás azonnal megjelenik a nyilvános eredményoldalon
- **Then** A rendszer validálja az adatokat (kötelező mezők, dátumformátum), és hibaüzenetet jelenít meg érvénytelen bevitel esetén

---

## US-06 — Regisztráció és bejelentkezés (Minden felhasználó)
**Leírás:**
Egy új látogató regisztrálni szeretne a platformra, hogy kommentelni, szavazni és személyre szabott tartalmat fogyaszthasson.

**Elfogadási kritériumok:**
- **Given** A látogató megnyitja a regisztrációs oldalt
- **When** Kitölti a szükséges adatokat (felhasználónév, e-mail, jelszó), elfogadja a feltételeket és elküldi az űrlapot
- **Then** A fiók létrejön, a felhasználó automatikusan bejelentkezik és átirányításra kerül a főoldalra
- **Then** A jelszó biztonságosan hashelve kerül tárolásra; az e-mail cím egyediségét a rendszer ellenőrzi
- **Then** Visszatérő felhasználó e-maillel és jelszóval be tud jelentkezni; hibás adatnál értelmes hibaüzenet jelenik meg

---

# 🚀 Stretch Goals (Opcionális)

## SG-01
- Valós idejű értesítések (Angular Snackbar): komment és szavazás értesítések push alapon

## SG-02
- Keresési funkció: hírek és eredmények keresése kulcsszó alapján

## SG-03
- Felhasználói profil oldal: saját kommentek, szavazások és aktivitás megtekintése

---

# ⚠️ Korlátok

## ⏱️ Idő
- Korlátozott fejlesztési idő (~600 óra)
- Fix beadási határidők betartása kötelező

## 🖥️ Platform és Technológiai Stack

| Réteg      | Technológia                          |
|------------|--------------------------------------|
| Frontend   | Angular 21                           |
| Backend    | ASP.NET Core 9                       |
| Adatbázis  | PostgreSQL                           |
| ORM        | Entity Framework Core (Migrations)   |
| Platform   | Web alapú alkalmazás                 |

## 🔌 Külső függőségek
- Nincs kötelező külső API-integráció
- Az adatok kezdetben manuálisan kerülnek seed-elésre az adatbázisba
- Nagy léptékű, valós idejű adatbevitel nem szükséges az MVP keretein belül

---

## 📊 Adatok
- Kezdeti adatok manuálisan seed-elve
- Nincs nagy léptékű, valós idejű adatbetöltés

---

# ✅ Definition of Done (Átadási Kritériumok)

A projekt akkor tekinthető késznek, ha az alábbi feltételek teljesülnek:

## Funkcionalitás
- Az összes MVP sztori (US-01 – US-06) implementálva és tesztelve
- Core user flow működik: `regisztráció → bejelentkezés → hír létrehozása`
- Szavazás és kommentelés funkcionálisan működik
- Eredmények és bajnokságkezelés Editor jogosultsággal elérhető, és helyesen működik

## Technikai
- Backend REST API stabilan elérhető és minden endpoint működik
- Az adatbázis séma Entity Framework migrációkkal leimplementálva
- Az Angular frontend közvetlenül a backend API-n keresztül kommunikál
- JWT-alapú autentikáció implementálva (login, role-based access Authorize segítségével)

## Minőség
- Legalább 30 automatizált teszt (unit + integrációs) megírva
- Minden teszt sikeresen lefut
- Input validáció backend és frontend oldalon egyaránt jelen van

## DevOps
- Az alkalmazás README alapján ~15–20 perc alatt elindítható
- Összes környezeti változó (env) dokumentálva és `.env.example` fájlban megtalálható
- A repóban semmilyen secret vagy API-kulcs nem szerepel

## Dokumentáció
- Az összes dokumentum a `/docs` mappában megtalálható
- README tartalmazza: setup, futtatás, env változók, architektúra áttekintés

## Biztonság
- Autentikáció és autorizáció előre leimplementálva (OWASP Top 10 figyelembevételével)
- Érzékeny adatok (jelszó, token) nem szivárognak ki logokban vagy válaszokban
- Megbízható, auditált titkosítási könyvtárak alkalmazva
