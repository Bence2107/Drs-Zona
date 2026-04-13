# GUI / UX Dokumentáció

Motorsport hírportál és eredménykövető Angular + ASP.NET alkalmazás felhasználói felületének dokumentációja.

## Tartalom

| Fájl                                         | Leírás                                             |
|----------------------------------------------|----------------------------------------------------|
| [pageflow.png](./pageflow.png)               | Képernyő-térkép — az összes oldal és navigáció     |
| [pageflow.mmd](./pageflow.mmd)               | Szerkeszthető Mermaid forrás a pageflow-hoz        |
| [screens.csv](./screens.csv)                 | Minden képernyő strukturált leírása                |
| [journeys.md](./journeys.md)                 | Top 3 user journey lépésről lépésre                |
| [design_system.md](./design_system.md)       | Vizuális nyelv, színpaletta, tipográfia            |
| [self_assessment.md](./self_assessment.md)   | Önértékelés 1–5 skálán                             |
| [screenshots/](./screenshots/)               | Képernyőképek (S01_*.png konvenció)                |
| [journey1.mp4](./journey1.mp4)               | Screen recording — 1. user journey (opcionális)    |

## Képernyők összefoglalója

### Publikus oldalak (auth nélkül elérhetők)
- **S01** — Home (belépési pont)
- **S02** — News (hírlista)
- **S03** — Reviews (tesztlista)
- **S04** — Results / Standings (eredmények és állások)
- **S05** — Serie detail (sorozat részletek)
- **S06** — Article detail (cikk oldal)

### Auth oldalak
- **S07** — Login / Register (csak vendégnek, guestGuard)
- **S08** — Profile (csak bejelentkezve, authGuard)

### Admin oldalak — editorGuard
- **S09** — Championships
- **S10** — Participations
- **S11** — Drivers
- **S12** — Constructors
- **S13** — Contracts
- **S14** — Entry list
- **S15** — Entry detail
- **S16** — Entry create

### Admin oldalak — authorGuard
- **S17** — Article create
- **S18** — Article update

### Modális dialógusok (♦)
- **D01** — Championship create
- **D02/D03** — Driver create / edit
- **D04/D05** — Constructor create / edit
- **D06/D07** — Contract create / edit
- **D08** — Poll add
- **D09** — Poll vote
- **D10** — Grand Prix create
- **D11** — Confirm dialog
- **D12** — Participation add

## Beadási checklist

- [x] pageflow.png + pageflow.mmd
- [x] Minden képernyőhöz screenshot a screenshots/ mappában, S## konvencióval
- [x] screens.csv minden képernyőre kitöltve
- [x] journeys.md — top 3 user journey
- [x] design_system.md
- [x] self_assessment.md táblázat + szövegrész
- [x] (Ajánlott) screen recording vagy GIF a fő journey-ről
- [ ] (Opcionális) mockup / Figma link
- [ ] (Opcionális) inspirations
- [ ] PR megnyitva `docs(ux): GUI/UX dokumentáció` címmel
