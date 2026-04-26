# Prompt Log

> A legfontosabb AI interakciók naplója. Nem minden prompt szerepel, csak a döntéseket
> befolyásoló vagy nagyobb kódot generáló esetek.

---

## P-01 – EK diagram validálás

- **Dátum:** 2025-12-16
- **Cél:** A bajnokság rendszer adatmodelljének (EK diagram) helyességének ellenőrzése
- **Prompt (kivonata):** "Megcsináltam az EK diagramot a bajnokság rendszerhez. Megfelelő-e a kapcsolatok és entitások szempontjából"
- **AI válasz kivonata:** Megerősítette a struktúrát, javasolta a `driver_participations` és `constructor_competitions` kapcsolótáblák explicit megjelölését
- **Hová került:** `docs/03_design/data_model.md`, DB séma
- **Módosítás:** Elfogadtam, nem változtattam

---

## P-02 – DTO generálás

- **Dátum:** 2026-02-09
- **Cél:** Az entity modellek alapján DTO osztályok legenerálása
- **Prompt (kivonata):** "Az alábbi entity osztályok alapján generáld le a szükséges DTO-kat validációs attribútumokkal"
- **AI válasz kivonata:** Legenerálta az összes DTO-t `[Required]`, `[MaxLength]`, `[Range]` attribútumokkal
- **Hová került:** `DTOs/` mappa, minden entity mellé
- **Módosítás:** Több helyen módosítottam – felesleges mezők eltávolítva, elnevezési konvenciók javítva, néhány validációs szabály pontosítva

---

## P-03 – Oldalsáv (sidebar) első implementáció

- **Dátum:** 2026-01-12
- **Cél:** Angular oldalsáv komponens megvalósítása
- **Prompt (kivonata):** "Készítsd el az Angular oldalsáv komponenst a következő struktúra alapján..."
- **AI válasz kivonata:** Elkészítette a sidebar komponenst routing integrációval
- **Hová került:** `src/app/app.component/`
- **Módosítás:** Az első verzió nem felelt meg a végleges use case-nek – tudatos döntés volt elhalasztani, később újradefiniálva és javítva (lásd P-08)

---

## P-04 – NuGet verzióhiba debug

- **Dátum:** 2026-03-03
- **Cél:** `dotnet build` warningok és verzióütközések megoldása
- **Prompt (kivonata):** "A következő build warningokat kapom dotnet build futtatásakor: [warning szöveg]. Mi lehet az oka?"
- **AI válasz kivonata:** Javasolta a `<PackageReference>` verziók manuális felülírását a `.csproj`-ban
- **Hová került:** Nem került be – az AI javaslata nem oldotta meg a problémát
- **Módosítás:** Saját debug: a valódi ok egy automatikusan rosszul letöltött NuGet csomag volt: `dotnet nuget locals all --clear` + újratelepítés oldotta meg

---

## P-05 – Dialógusok tervezése

- **Dátum:** 2026-03-10
- **Cél:** Angular Material dialógus komponensek tervezése a modellek és DTO-k alapján
- **Prompt (kivonata):** "A következő model és DTO struktúra alapján tervezd meg az Angular Material dialógus komponenseket"
- **AI válasz kivonata:** Legenerálta a dialógus komponenseket reaktív formákkal, validációs megjelenítéssel
- **Hová került:** `src/app/components/`
- **Módosítás:** Jóváhagytam, kisebb style módosítások

---

## P-06 – Record vs. class összehasonlítás

- **Dátum:** 2026-01-25
- **Cél:** Döntés: C# record vagy class használata DTO-khoz és response objektumokhoz
- **Prompt (kivonata):** "Miben különbözik a C# record és a class? DTO-khoz és ResponseResult-hoz melyiket érdemes használni?"
- **AI válasz kivonata:** Ismertette a különbségeket (immutabilitás, value equality, `with` expression); record-ot javasolt DTO-khoz
- **Hová került:** Döntés dokumentálva, `ResponseResult<T>` implementáció
- **Módosítás:** Én döntöttem a record mellett: nem volt nagyobb különbség class és record között, az EF Core pedig hibák nélkül működött ezzel.

---

## P-07 – SQL adatok generálása

- **Dátum:** 2026-02-14
- **Cél:** Teszt/seed adatok generálása SQL INSERT formátumban a séma alapján
- **Prompt (kivonata):** "A következő séma és eredménytáblázat alapján generáld le az SQL INSERT szkripteket"
- **AI válasz kivonata:** Legenerálta az összes INSERT-et UUID-kkal, timestampokkal, idegen kulcsokkal
- **Módosítás:** Több helyen javítottam – UUID formátum hibák, timestamp timezone eltérések, FK sorrend problémák

---

## P-08 – Oldalsáv javítás

- **Dátum:** 2026-03-27
- **Cél:** Az első oldalsáv implementáció újradefiniálása és javítása
- **Prompt (kivonata):** "Az oldalsáv komponenst újra kell definiálni. Az új követelmények: [lista]. Javítsd a meglévő implementációt"
- **AI válasz kivonata:** Átírta a sidebar-t az új routing struktúrával és active state kezeléssel
- **Hová került:** `src/app/app.component/` – frissített verzió
- **Módosítás:** Kisebb módosítások, elfogadva

---

## P-09 – Grand Prix zászló pipe

- **Dátum:** 2026-03-22
- **Cél:** Angular pipe készítése, amely a Grand Prix neve alapján kiírja az ország zászlóját
- **Prompt (kivonata):** "Készíts Angular pipe-ot, amely a Grand Prix neve alapján visszaadja a megfelelő emoji zászlót"
- **AI válasz kivonata:** Legenerálta a pipe-ot egy map objektummal (GP név → emoji zászló)
- **Hová került:** `src/app/pipes/country-flag.pipe.ts`
- **Módosítás:** Néhány hiányzó GP hozzáadva, egy-két hibás leképezés javítva

---

## P-10 – Backend metódusok async-é alakítása

- **Dátum:** 2026-03-11
- **Cél:** Az összes backend service metódus átalakítása szinkronból aszinkronra
- **Prompt (kivonata):** "Írd át az összes service metódust async/await-re, Task<ResponseResult<T>> visszatérési értékkel"
- **AI válasz kivonata:** Átírta az összes metódust, `await` hívásokkal, `ConfigureAwait` nélkül
- **Hová került:** Összes `Services/` fájl
- **Módosítás:** Néhány helyen foltozás kellett – hiányzó `await`, helytelenül async-é tett szinkron metódusok

---

## P-11 – Hírek oldalazás és szűrés

- **Dátum:** 2026-03-30
- **Cél:** Angular Material paginator és szűrő hozzáadása a hírek listához
- **Prompt (kivonata):** "Angular Material segítségével illessz be oldalazást és tag alapú szűrést a hírek komponensbe"
- **AI válasz kivonata:** Implementálta a `MatPaginator` és `MatSelect` szűrőt reaktív formával
- **Hová került:** `src/app/articles/pages/news/` és `src/app/articles/pages/reviews/`
- **Módosítás:** Elfogadva, sikeresen végrehajtva

---

## P-12 – WYSIWYG könyvtár kiválasztás

- **Dátum:** 2025-03-26
- **Cél:** Rich text editor keresése Angular projekthez
- **Prompt (kivonata):** "Van-e jó WYSIWYG külső könyvtár Angular projekthez, ami könnyen integrálható?"
- **AI válasz kivonata:** Ajánlotta a kolkov/angular-editor, ismertette az előnyeit és az integrációs lépéseket
- **Hová került:** `package.json`, cikk szerkesztő komponens
- **Módosítás:** Licenc manuálisan ellenőrizve (MIT), elfogadva és integrálva

---

## P-13 – Backend tesztek generálása

- **Dátum:** 2026-04-15
- **Cél:** A teljes backend service és controller réteg xUnit tesztjeinek generálása
- **Prompt (kivonata):** "A teljes backend service és controller implementáció alapján generálj xUnit unit és integrációs teszteket"
- **AI válasz kivonata:** Legenerálta az összes tesztosztályt – unit tesztek mock repository-val, integrációs tesztek in-memory DB-vel, controller tesztek WebApplicationFactory-val
- **Hová került:** `Tests/Services/Units/`, `Tests/Services/Integrations/`, `Tests/Controllers/`
- **Módosítás:** Edge case-ek pótolva, néhány assertion pontosítva; végeredmény: 396 teszt, 396 passed
