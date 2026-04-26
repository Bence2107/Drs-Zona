# Product Vision — Drs-Zóna

## 🏁 Probléma és Kontextus

A jelenlegi motorsport-tematikájú weboldalak fragmentáltak: a hírek, eredmények és közösségi interakció külön platformokon érhető el. Ennek következménye az alacsony felhasználói elköteleződés, az egységes közösségi élmény hiánya, valamint az újságírói szemléletek és a rajongói aktivitás szétszórtsága.

> **Megoldandó probléma:** Nincs egyetlen, centralizált platform, amely egyaránt kiszolgálja az újságírókat (tartalom-előállítás), az olvasókat (hírek, eredmények, közösség) és a szerkesztőket (adatkezelés) — egy modern, kényelmes környezetben.

---

## 🎯 Vízió

> A **DRS-Zóna** egy motorsport-tematikájú, centralizált közösségi platform, ahol az újságírók könnyedén hozhatnak létre tartalmakat, a rajongók olvashatnak híreket, követhetik az eredményeket, kommentelhetnek és szavazhatnak — mindezt egyetlen, modern, mobilbarát felületen.

---

## 👤 Célfelhasználók (Personák)

### ✍️ Vivien — Sportújságíró

**Szegmens:** Olyan fejlesztői környezetre van szüksége, amely megkönnyíti a releváns és érdekes tartalom előállítását.

**Szükségletei:**
- Azonnali visszajelzés az olvasóktól (kommentek, szavazások)
- Könnyen kezelhető, rugalmas tartalomkezelő rendszer (CMS-szerű editor)
- Eredmények hozzárendelése a cikkekhez (pl. futamhoz kapcsolódó hír)
- Szerkesztési és törlési lehetőség a közzétett tartalmakhoz

**Szerepköre a rendszerben:** `Writer` — Híreket és cikkeket hozhat létre, szerkeszthet és tehet közzé.

---

### 🏎️ Bence — Motorsport rajongó

**Szegmens:** Az olvasó részesévé akar válni a közösségnek, aktívan részt kíván venni a motorsport diskurzusban.

**Szükségletei:**
- Témaszerinti szűrés (több sportág: F1, WRC, WEC, MotoGP)
- Könnyen navigálható menü, letisztult dizájn, mobilbarát felület (UI/UX)
- Lehetőség kommentelésre és szavazásokon való részvételre
- Bajnokságok és versenyeredmények böngészése sportágak szerint

**Szerepköre a rendszerben:** `User` — Olvasás, kommentelés, szavazás.

---

### 📊 Attila — Eredményszerkesztő

**Szegmens:** Pontos és naprakész motorsport-adatokat szeretne adminisztrálni; bajnokságokat, versenyzőket, konstruktőröket és szerződéseket kezel.

**Szükségletei:**
- Strukturált adminisztrációs felület bajnokság- és eredménykezeléshez
- Versenyzők, konstruktőrök és szerződések CRUD-műveletei
- Validáció és visszajelzés hibás adatbevitel esetén
- Az adatbázis-módosítások azonnal tükröződjenek a nyilvános felületen

**Szerepköre a rendszerben:** `Editor` — Teljes adatkezelési jogosultság a bajnokság-, versenyző- és eredménymodulokban.

---

## 💎 Értékajánlat

A DRS-Zóna egyetlen, modern platformon egyesíti:
- az **újságírók** számára: kényelmes tartalom-előállítást és azonnali olvasói visszajelzést
- az **olvasók** számára: naprakész híreket, bajnokság-eredményeket és közösségi részvételt
- a **szerkesztők** számára: strukturált, validált adatkezelési felületet

Minden egy helyen, kényelmes, modern környezetben — legyen szó F1-ről, WRC-ről, MotoGP-ről vagy WEC-ről.

---

## 🚫 Nem célok (Non-Goals)

- Minden motorsportág eredményeinek automatikus, API-alapú szinkronizálása — sportáganként eltérő adatstruktúrák és hiányzó nyílt API-k miatt
- Forradalmi, teljesen új funkciók implementálása — a cél a bevált megoldások egységesítése
- Mobilalkalmazás (natív iOS/Android) — a platform webalapú, de reszponzív tervezéssel
- Fizetős tartalom vagy előfizetési modell kialakítása az MVP keretein belül

---

## ⚡ Kockázatok és Bizonytalanságok

| Kockázat | Hatás | Mérséklés |
|---|---|---|
| Scope creep | Nem teljesíthető MVP határidőre | Scope Contract betartása, rendszeres sprint review |
| Technológia elévülése | Nehéz karbantarthatóság | Elterjedt, aktívan fejlesztett stack (Angular, .NET) |
| OWASP Top 10 sebezhetőségek | Adatszivárgás, jogosulatlan hozzáférés | Auditált könyvtárak, input validáció, JWT auth |
| „Működik nálam" szindróma | Nem reprodukálható hibák | Tesztelés min. 2 különböző eszközön |
