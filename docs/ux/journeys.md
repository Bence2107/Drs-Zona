# User Journeys — Top 3 felhasználói feladat

---

## Journey 1 — Cikk olvasása és kommentelése

**Persona:** Kovács Péter, F1-rajongó, rendszeres olvasó. Értesítést kapott egy új versenyeredmény-cikkről, el akarja olvasni és véleményt fűzni hozzá.

**Belépési pont:** Push notification / direct link (deep link: `/article/:slug`)

| # | Képernyő | Mit csinál a user | Mit lát / rendszerválasz | Lehetséges hibaág |
| --- | ---------- | ------------------- | -------------------------- | ------------------- |
| 1 | S01 — Home | Megnyitja az appot | Főoldal tölt, vagy azonnal az article oldalra kerül deep link esetén | Hálózati hiba → error állapot, újrapróbálkozás gomb |
| 2 | S06 — Article detail | Olvassa a cikket, legörgeti az oldalt | Cikk szövege, képek, a kommentszekció betöltődik | Cikk nem található → 404, redirect S01-re |
| 3 | S06 — Article detail | Rákattint a „Komment írása" inputra | Ha nincs bejelentkezve → átirányítás S07-re; ha be van → a komment form aktívvá válik | Nincs bejelentkezve: login oldal nyílik |
| 4 | S07 — Login | Beírja az e-mail-t és a jelszót és submit | Sikeres bejelentkezés → visszanavigál S01-re | Hibás jelszó → error üzenet; elfelejtett jelszó |
| 5 | S06 — Article detail | Beírja a komment szövegét, rákattint a „Küldés" gombra | A komment megjelenik a listában (success snackbar) | Üres komment → disabled gomb; szerver hiba → error snackbar |

**Sikerkritérium:** A komment megjelenik a listában, a user látja a saját nevével.

**Mért időtartam:** ~45–90 mp (ha már be van jelentkezve: ~20 mp), ~6–8 kattintás.

---

## Journey 2 — Bajnokság-eredmény megnézése

**Persona:** Nagy Anna, alkalmi érdeklődő. Látni szeretné az aktuális pilóta-bajnokság állását.

**Belépési pont:** Böngésző, beírja az URL-t, vagy a főoldalról navigál.

| # | Képernyő | Mit csinál a user | Mit lát / rendszerválasz | Lehetséges hibaág |
| --- | ---------- | ------------------- | -------------------------- | ------------------- |
| 1 | S01 — Home | Megnyitja az oldalt | Főoldal betölt, navigációs sáv látható | Hálózati hiba → error állapot |
| 2 | S04 — Results / Standings | Rákattint a „Results" menüpontra | Standings oldal tölt, bajnokság-szűrő és az alapértelmezett szezon állása jelenik meg | Nincs adat → empty állapot üzenettel |
| 3 | S04 — Results / Standings | A szűrőből kiválasztja a kívánt szezont / sorozatot | A standings táblázat frissül az adott bajnokság adataival (loading majd success) | Érvénytelen szűrő paraméter → fallback az első elérhető szezonra |
| 4 | S04 — Results / Standings | Vált a Egyéni" és „Csapat" tab között | A táblázat átváltja a megjelenített adatokat | — |
| 5 | S05 — Serie detail | Rákattint egy sorozat nevére a sidenavon | Sorozat oldal tölt a kapcsolódó cikkekkel | Sorozat nem létezik → Server Error State |

**Sikerkritérium:** A user látja a kívánt szezon pilóta-bajnokság állástábláját.

**Mért időtartam:** ~20–40 mp, ~3–5 kattintás.

---

## Journey 3 — Új cikk közzététele (admin / author)

**Persona:** Szerkesztő Balázs, a csapat szerzője. Megírta a legfrissebb verseny összefoglalóját, és fel akarja tölteni az oldalra.

**Belépési pont:** Bejelentkezés után az admin navigációs linkre kattint.

| # | Képernyő | Mit csinál a user | Mit lát / rendszerválasz | Lehetséges hibaág |
| --- | ---------- | ------------------- | -------------------------- | ------------------- |
| 1 | S07 — Login | Bejelentkezik szerzői (vagy admin) fiókjával | Sikeres auth → S01-re kerül. | Hibás hitelesítők → error üzenet |
| 2 | S01 — Home | Rákattint az Hírek navbarra (S2). | Lát egy + gombot, ami a elviszi a (`/admin/articles/create`) oldalhoz | authorGuard → ha nincs jog, redirect S02-re |
| 3 | S17 — Article create | Kitölti a cím, slug, kategória mezőket; megírja a szöveget a rich text szerkesztőben | Az editor betölt, a form interaktív | Szerkesztő JS hiba → error display komponens |
| 4 | S17 — Article create | Feltölt egy borítóképet | Kép előnézet megjelenik; méret/formátum validáció fut | Túl nagy fájl → validációs hibaüzenet; sikertelen feltöltés → error snackbar |
| 5 | S17 — Article create | Rákattint a „Mentés / Közzététel" gombra | Loading; sikeres mentés → snackbar + redirect S06-ra (az cikkek oldalára) | Slug már foglalt → inline hibaüzenet a slug mezőnél; hálózati hiba → Mentés gomb nem fog működni |

**Sikerkritérium:** A cikk elérhető a `/article/:slug` útvonalon, megjelenik a News vagy Reviews listában.

**Mért időtartam:** ~5–15 perc (írási idő nélkül: ~3–4 perc), ~10–15 interakció.
