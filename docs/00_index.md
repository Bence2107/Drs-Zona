# Project Documentation Index

> **Note:** This documentation is written in Hungarian as it is a mandatory requirement for a university thesis project. The technical terms and the structure follow international software engineering standards.

---

# Projekt Dokumentáció - Tartalomjegyzék

Ez a dokumentum a szakdolgozathoz tartozó szoftverprojekt központi indexe, amely összefogja a projekt életciklusának különböző szakaszait a tervezéstől az üzemeltetésig.

## 📂 01_product (Termék és Keretrendszer)
Ebben a mappában a projekt üzleti és funkcionális alapvetései találhatók.
* **vision.md**: A projekt átfogó jövőképe.
    * *Kiindulópont:* Definiáld a megoldandó problémát, a célcsoportot és a fő értékajánlatot.
* **scope_contract.md**: A projekt hatóköre és keretszerződése.
    * *Kiindulópont:* Sorold fel a vállalt funkciókat (In-scope) és azokat, amik nem részei a fejlesztésnek (Out-of-scope).

## 📂 02_architecture (Architektúra)
A rendszer technikai felépítését és a fontosabb döntéseket tartalmazó rész.
* **quality_attributes**: A rendszer minőségi elvárásai (pl. teljesítmény, biztonság, skálázhatóság).
    * *Kiindulópont:* Határozd meg a válaszidőkre vagy a rendelkezésre állásra vonatkozó elvárásokat.
* **adr/**: Architectural Decision Records (5 db fájl).
    * *Kiindulópont:* Rögzítsd itt a legfontosabb döntéseket (pl. technológiai stukk választása, adatbázis típus, autentikációs mód). Minden ADR-nek legyen kontextusa, döntése és következménye.

## 📂 03_design (Részletes Tervezés)
A megvalósításhoz szükséges technikai specifikációk.
* **api.html**: Az API végpontok interaktív vagy statikus dokumentációja.
    * *Kiindulópont:* Swagger/OpenAPI specifikáció alapján készült végpontleírások.
* **schema.sql**: Az adatbázis fizikai sémája.
    * *Kiindulópont:* A táblák létrehozásáért felelős SQL scriptek és indexek.
* **data_model.md**: Logikai adatmodell leírása.
    * *Kiindulópont:* ER-diagram és az entitások közötti kapcsolatok magyarázata.
* **error_handling.md**: Egységes hibakezelési stratégia.
    * *Kiindulópont:* Saját hibakódok, HTTP státuszkódok használata és hibaüzenet formátumok.

## 📂 04_quality (Minőségbiztosítás)
A szoftver tesztelésével kapcsolatos dokumentumok.
* **test_strategy.md**: Általános tesztelési megközelítés.
    * *Kiindulópont:* Unit, integrációs és E2E tesztek aránya és módszertana.
* **test_report.md**: Szöveges összefoglaló a tesztelési fázisról.
    * *Kiindulópont:* Tesztelési lefedettség és a talált/javított hibák összefoglalása.
* **test_results.trx**: Automatikus tesztfuttatások eredményfájlja.
    * *Kiindulópont:* CI/CD folyamat által generált technikai riport.

## 📂 05_security_ops (Biztonság és Üzemeltetés)
Az alkalmazás biztonságos futtatásához szükséges információk.
* **deploy_runbook.md**: Telepítési útmutató.
    * *Kiindulópont:* Lépésről lépésre követhető instrukciók a környezet felállításához.
* **observability.md**: Monitorozás és naplózás.
    * *Kiindulópont:* Milyen metrikákat figyelünk és hol érhetők el a logok.
* **privacy_licensing.md**: Adatvédelem és licencek.
    * *Kiindulópont:* Felhasznált külső könyvtárak licencei és GDPR megfelelőségi nyilatkozat.
* **threat_model.md**: Fenyegetettségi modell.
    * *Kiindulópont:* Potenciális támadási felületek és az ellenük tett óvintézkedések.

## 📂 07_ai (Mesterséges Intelligencia)
Az AI asszisztált fejlesztés dokumentálása.
* **ai_manifest.md**: Az AI használatának szabályai és eszközei a projektben.
* **prompt_log.md**: A fejlesztés során használt kulcsfontosságú utasítások (promptek) gyűjteménye.
* **verification_log.md**: Az AI által generált kódok ellenőrzésének naplója.

## 📂 ux (Felhasználói Élmény)
* **README.md**: A felhasználói felület tervezésének folyamata.
    * *Kiindulópont:* Wireframe-ek, design system és a felhasználói útvonalak (User flows) leírása.