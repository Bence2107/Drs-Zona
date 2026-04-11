# Alkalmazás Képernyők és Dialógusok (Sitemap)

## Oldalak (Screens)

| ID | Név | Cél | Belépési pont | Auth? | Fő interakciók | Adatforrások | Validációk | Állapotok | A11y |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **S01** | Home | Az alkalmazás főoldala – legfrissebb hírek és navigáció | induló / URL gyökér | nem | navigáció a hírek / vélemények / eredmények felé; cikk kártya kattintás | GET /api/articles?type=news&limit=N; GET /api/articles?type=review&limit=N | nincs form | loading / empty / error | - |
| **S02** | News | Hírcikkek listája lapozással | S01 nav link vagy /news | nem | cikk kártya kattintás; lapozó; esetleges szűrő | GET /api/articles?type=news | nincs form | loading / empty / error | article list szerepkör; kártya title olvasható |
| **S03** | Reviews | Tesztek és értékelések listája | S01 nav link vagy /reviews | nem | cikk kártya kattintás; lapozó | GET /api/articles?type=review | nincs form | loading / empty / error | article list szerepkör |
| **S04** | Results / Standings | Bajnokság-eredmények és állások megtekintése | S01 nav link vagy /results | nem | szűrő; pilóta- és konstruktőr-tab váltás; körtáblázat | GET /api/standings; GET /api/championships; GET /api/circuits | nincs form | loading / empty / error | table role; column headers scope |
| **S05** | Serie detail | Egy adott sorozat (pl. F1 / F2) részletes oldala | S01 cikk kártya vagy /serie/:name | nem | sorozathoz tartozó cikkek böngészése; back gomb | GET /api/series/:name; GET /api/articles?serie=:name | nincs form | loading / empty / error | heading hierarchia; link title |
| **S06** | Article detail | Egy cikk teljes szövege kommentekkel és szavazásokkal | S02 / S03 kártya; deep link; S17-S18 mentés után | nem | komment írása; szavazás; share; poll interakció; szerkesztés | GET /api/articles/:slug; GET /api/comments/:articleId; GET /api/polls/:articleId | komment: min 3 kar.; üres komment tiltott | loading / empty / error / success | article landmark; aria-live a szavazásnál |
| **S07** | Login / Register | Felhasználói bejelentkezés és regisztráció | nav login gomb; /auth (guestGuard) | nem (guest-only) | tab váltás login ↔ register; e-mail + jelszó form; submit | POST /api/auth/login; POST /api/auth/register | email: valid; jelszó: min 8 kar.; confirm megegyezés | loading / error / success | form labels; aria-invalid; error role="alert" |
| **S08** | Profile | Bejelentkezett felhasználó profilja | nav profil ikon; /profile/:username | igen (authGuard) | profil szerkesztés; avatar feltöltés; kommentek/szavazatok listája | GET /api/users/:username; GET /api/comments; GET /api/polls | Edit: username min 3 kar.; e-mail formátum | loading / empty / error / success | tab role; tabpanel; - |
| **S09** | Championships (admin) | Bajnokságok kezelése – lista és létrehozás | /admin/championships | igen (editorGuard) | lista megtekintés; + Létrehozás (D01); törlés (D11) | GET /api/championships | nincs form az oldalon | loading / empty / error | table aria; button aria-label |
| **S10** | Participations (admin) | Bajnoksághoz tartozó résztvevők | /admin/participations/:champId | igen (editorGuard) | bajnokság-szűrő; + Hozzáadás (D12); törlés | GET /api/participations?champId=:id | nincs form az oldalon | loading / empty / error | select aria-label; table scope |
| **S11** | Drivers (admin) | Pilóták adatainak kezelése | /admin/drivers | igen (editorGuard) | pilóta lista; + Létrehozás (D02); szerkesztés (D03); törlés (D11) | GET /api/drivers | nincs form az oldalon | loading / empty / error | icon button aria-label |
| **S12** | Constructors (admin) | Csapatok adatainak kezelése | /admin/constructors | igen (editorGuard) | csapat lista; + Létrehozás (D04); szerkesztés (D05); törlés (D11) | GET /api/constructors | nincs form az oldalon | loading / empty / error | icon button aria-label |
| **S13** | Contracts (admin) | Pilóta–csapat szerződések kezelése | /admin/contracts | igen (editorGuard) | szerződés lista; + Létrehozás (D06); szerkesztés (D07); törlés (D11) | GET /api/contracts | nincs form az oldalon | loading / empty / error | table aria |
| **S14** | Entry list (admin) | Futameredmény-bejegyzések listája | /admin/results/entry | igen (editorGuard) | lista; sor kattintás → S15; futam-szűrő | GET /api/entries | nincs form az oldalon | loading / empty / error | link aria-label soronként |
| **S15** | Entry detail (admin) | Futam részletes eredménytáblája | S14 sor kattintás; /admin/results/entry/:gpId | igen (editorGuard) | eredmény szerkesztés; mentés; + Létrehoz → S16 | GET /api/entries/:gpId; GET /api/drivers; GET /api/constructors | pozíció: szám; idő: formátum | loading / error / success | input labels |
| **S16** | Entry create (admin) | Új futameredmény létrehozása | S15 Létrehoz gomb; /admin/.../:gpId/create | igen (editorGuard) | teljes eredmény form; mentés; mégse | POST /api/entries | kötelező mezők; érvényes értékek | loading / error / success | form labels |
| **S17** | Article create (admin) | Új cikk írása | /admin/articles/create | igen (authorGuard) | rich text editor; cím; slug; kategória; kép feltöltés; mentés | POST /api/articles; GET /api/series | cím: min 5 kar.; slug: unique; kép: max méret | loading / error / success | editor aria-label; - |
| **S18** | Article update (admin) | Meglévő cikk szerkesztése | S06 szerkesztés gomb; /admin/articles/update/:slug | igen (authorGuard) | ugyanaz mint S17 – előre töltött adatokkal | GET /api/articles/:slug; PUT /api/articles/:slug | ugyanaz mint S17 | loading / error / success | ugyanaz mint S17 |

---

## Dialógusok (Dialogs)

| ID | Név | Cél | Belépési pont | Auth? | Fő interakciók | Adatforrások | Validációk | Állapotok | A11y |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **D01** | Champ create dialog | Új bajnokság gyors létrehozása | S09 + gomb | igen | név; év; sorozat; mentés | POST /api/championships | név: kötelező; év: 4 jegy | loading / error / success | dialog; - |
| **D02** | Driver create dialog | Új pilóta felvitele | S11 + gomb | igen | név; dátum; nemzet; szám; mentés | POST /api/drivers | kötelezőek; szám: 1–99 | loading / error / success | dialog; - |
| **D03** | Driver edit dialog | Pilóta adatainak szerkesztése | S11 szerkesztés ikon | igen | előre töltött form; mentés | PUT /api/drivers/:id | ugyanaz mint D02 | loading / error / success | dialog; - |
| **D04** | Constructor create dialog | Új konstruktőr felvitele | S12 + gomb | igen | csapatnév; ország; mentés | POST /api/constructors | név: kötelező | loading / error / success | dialog; - |
| **D05** | Constructor edit dialog | Meglévő csapat szerkesztése | S12 szerkesztés ikon | igen | előre töltött form; mentés | PUT /api/constructors/:id | ugyanaz mint D04 | loading / error / success | dialog; - |
| **D06** | Contract create dialog | Pilóta–csapat szerződés | S13 + gomb | igen | pilóta; csapat; szezon; mentés | POST /api/contracts | mindkét select kötelező | loading / error / success | dialog; select aria-label |
| **D07** | Contract edit dialog | Szerződés szerkesztése | S13 szerkesztés ikon | igen | előre töltött form; mentés | PUT /api/contracts/:id | ugyanaz mint D06 | loading / error / success | dialog; - |
| **D08** | Poll add dialog | Új szavazás cikkhez | S06 poll szekció + gomb | igen | kérdés; válaszlehetőségek; mentés | POST /api/polls | kérdés kötelező; min 2 válasz | loading / error / success | dialog; - |
| **D09** | Poll vote dialog | Szavazás leadása | S06 szavazás gomb | igen | opció kiválasztás; mentés | POST /api/polls/:id/vote | opció kötelező | loading / error / success | radio group aria; dialog |
| **D10** | GP create dialog | Új nagydíj létrehozása | S06 vagy admin | igen | nagydíj neve; pálya; dátum; mentés | POST /api/grands-prix | név és dátum kötelező | loading / error / success | dialog; datepicker aria |
| **D11** | Confirm dialog | Megerősítés törléshez | törlés gomb bárhol | igen | Igen / Mégse gombok | szülő kezeli | nincs form | nincs állapot | dialog; aria-describedby |
| **D12** | Participation add dialog | Résztvevő bajnoksághoz | S10 + gomb | igen | csapat; szezon; mentés | POST /api/participations | select kötelező | loading / error / success | dialog; - | 