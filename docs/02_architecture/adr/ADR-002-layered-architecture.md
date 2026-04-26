# ADR-002: Réteges architektúra (Controller → Service → Repository)

- **Dátum:** 2026-02-01
- **Státusz:** Accepted

---

## Context

A backend struktúráját meg kellett határozni. A projekt egy motorsport adatokat kezelő
alkalmazás, ahol sok entity van (Driver, Constructor, GrandPrix, Results, stb.) és
komplex üzleti logika (pontszám újraszámítás, bajnokság aggregálás). Az üzleti logika
tesztelhetősége kulcsfontosságú.

Kényszerek:
- Egyedül fejlesztett projekt, egyszerűen átlátható struktúra kell
- Az üzleti logika izoláltan, DB nélkül tesztelhető legyen
- AI-asszisztált fejlesztés: konzisztens, előre jelezhető fájlstruktúra és komponesegíti az AI-t

---

## Decision

Controller → Service → Repository háromrétegű architektúrát alkalmazunk;
az üzleti logika a Service rétegben él, a Controller csak HTTP-t service-t,
a Repository csak adathozzáférés, tényleges művelet. A Service-ben található meg a komplex logika.

---

## Alternatívák

**A) Clean Architecture (Domain / Application / Infrastructure)**
- Erős szétválasztás, dependency inversion
- Nagyobb boilerplate (interfaces mindenhol, mapping rétegek)
- Egyedül fejlesztett kis projekten túlzott overhead

**B) Minimal API + direkt DB hozzáférés (no service layer)**
- Gyors prototípus
- Tesztelhetetlen, logika és infrastruktúra összefonódik
- Skálázás és karbantartás nehéz

---

## Következmények

**Pozitív:**
- Átlátható, minden fejlesztő (és AI) azonnal érti a struktúrát
- Service réteg mock repository-val unit tesztelhető
- Controller vékony marad, csak HTTP concern-t kezel

**Negatív / kockázat:**
- Nincs strict dependency inversion – Service közvetlenül függhet konkrét implementációtól
- Nagy projekten a Service réteg "god class"-szá nőhet
- Domain logika és alkalmazáslogika nincs szétválasztva

---

## Verification

- 184 unit teszt fut Service rétegen, mock repository-val, DB nélkül
- 143 integrációs teszt ellenőrzi Service + Repository + DB együttműködését
- Minden tesztosztály a megfelelő réteg namespace-ében van (`Tests.Services.Units`, `Tests.Services.Integrations`)
