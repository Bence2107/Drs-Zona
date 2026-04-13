# Design System — Vizuális nyelv

## UI / Komponens könyvtár

- Angular Material (`@angular/material`)
- Saját CSS / SCSS

---

## Színpaletta

| Szerepkör      | Név               | Hex (light)                      | Hex (dark)                       | Felhasználás                  |
|----------------|-------------------|----------------------------------|----------------------------------|-------------------------------|
| Primary        | Főszín            | `#f8f4f4`                        | `#21212e`                        | Gombok, linkek, aktív állapot |
| Secondary      | Másodlagos        | `#eae3e3`                        | `#181924`                        | Kártyák, háttér elemek        |
| Accent         | Kiemelő           | `#f8274f`                        | `#f8274f`                        | Badge-ek, tag-ek              |
| Success        | Siker             | `#f8f4f4`                        | `#21212e`                        | Sikeres műveletek (snackbar)  |
| Warning        | Figyelmeztetés    | `#f8274f`                        | `#f8274f`                        | Figyelmeztetések              |
| Error / Danger | Hiba              | `#f8274f`                        | `#f8274f`                        | Validációs hibák, törlés      |
| Surface        | Felület           | `#f8f4f4`                        | `#21212e`                        | Kártyák, dialógusok háttere   |
| Background     | Alap háttér       | `Gradient(#F8F4F4FF, #EDEBEBFF)` | `Gradient(#27273BFF, #21212EFF)` | Oldal háttér                  |
| Text Primary   | Elsődleges szöveg | `#000000`                        | `#bbbbbb`                        | Főcímek, törzs szöveg         |
| Text Secondary | Másodlagos szöveg | `#606060`                        | `#878a8c`                        | Feliratok, placeholder        |

---

## Tipográfia

| Szerepkör | Font család | Méret | Font weight |
| ----------- | ------------- | ------- | ------------- |
| H1 — Főcím | 'Kanit', sans-serif; | 32 px | 700 |
| H2 — Alcím | 'Kanit', sans-serif; | 24 px | 600 |
| H3 — Szekcióím | 'Kanit', sans-serif; | 18.72 px | 500 |
| Body — Törzs | 'Kanit', sans-serif; | 16px | 400 |
| Small — Felirat | 'Kanit', sans-serif; | 12–14px | 1.4 |

---

## Spacing

- **Alap egység:** 4px vagy 8px (pl. 8px-alapú → 8, 16, 24, 32, 48, 64px)
- **Max content width:** TODO px (pl. 1200px vagy 1440px)
- **Oldalsó padding (mobile):** TODO px
- **Oldalsó padding (desktop):** TODO px

---

## Ikonkészlet

Példák:
- Material Icons (`mat-icon`)
- Google Icons
- FontAwesome

---

## Sötét mód

- **Támogatott:** igen

---

## Reszponzív breakpointok

| Breakpoint  | Min szélesség   | Leírás                     |
|-------------|-----------------|----------------------------|
| xs- sm      | 600/768 px      | Telefon fekvő / kis tablet |
| md          | 980 px          | Tablet                     |
| lg          | 1280 px         | Laptop / desktop           |

---
