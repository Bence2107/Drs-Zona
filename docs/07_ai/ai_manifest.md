# AI Manifest

## 1. Használt eszközök

| Eszköz               | Verzió / Platform           | Felhasználás                                 |
|----------------------|-----------------------------|----------------------------------------------|
| Claude (Anthropic)   | Claude.ai (webes felület)   | Kódgenerálás, tesztek, dokumentáció, debug   |
| Google Gemini        | Gemini webes felület        | Kiegészítő kérdések, összehasonlítások       |

---

## 2. Felhasználási területek

| Terület          | Részletek                                                                                    |
|------------------|----------------------------------------------------------------------------------------------|
| **Kódírás**      | Service-ek, controller-ek, repository-k, DTO-k, Angular komponensek, pipe-ok, async refaktor |
| **Tesztek**      | xUnit unit és integrációs tesztek generálása a teljes backend alapján                        |
| **Dokumentáció** | API dokumentáció, error handling, ADR-ek, threat model                                       |
| **Tervezés**     | EK diagram validálás, dialógus modellek tervezése, adatbázis séma review                     |

---

## 3. Tiltások – mit nem adtam ki az AI-nak

- **Jelszavak, connection string-ek, JWT secret** – soha nem szerepeltek promptban
- **Valódi felhasználói adatok** – tesztadatok generálásánál fiktív adatokat használtam
- **PII** – email címek, személyes adatok nem kerültek promptba
- A promptokban csak kódstruktúra, séma és logikai leírás szerepelt

---

## 4. Kritikus döntések ahol én döntöttem (nem az AI)

**1. Dependency verzióhiba megoldása**
Az AI adott egy javaslatot a `dotnet build` warningokra, de az nem oldotta meg a problémát.
Én jöttem rá, hogy a valódi ok egy automatikusan rosszul letöltött NuGet csomag verziója volt.
Az AI javaslata helytelen irányba vitt volna.

**2. Oldalsáv első verziójának elvetése**
Az AI megcsinálta az oldalsávot, de az első implementáció nem felelt meg a valódi use case-nek.
Tudatos döntés volt azt egy későbbi fázisra halasztani és újradefiniálni a követelményeket.

**3. DTO-k módosítása generálás után**
Az AI legenerálta a DTO-kat a modellek alapján, de több helyen módosítanom kellett –
felesleges mezők, nem megfelelő validációs attribútumok, elnevezési konvenciók.

**4. Record vs. class döntés**
Az összehasonlítás után én döntöttem a record használata mellett a DTO-knál,
mert a class implementációja hosszabb, és komplexebb dto-k esetén előnyösebb

**5. SQL adatbevitel manuális javítása**
Az AI generálta az SQL insert szkripteket a séma alapján, de néhány esetben
hibásan kezelte az UUID-kat és a timestamp formátumokat – ezeket én javítottam kézzel.

---

## 5. Kockázatok és kezelésük

| Kockázat                                           | Előfordult?                                    | Kezelés                                                       |
|----------------------------------------------------|------------------------------------------------|---------------------------------------------------------------|
| **Hallucináció** (nem létező API, hibás szintaxis) | Igen (loading container hiba nem oldódott meg) | Kézi debug, AI javaslat elvetve                               |
| **Hibás security tanács**                          | Nem fordult elő kritikusan                     | JWT és auth döntések ADR-ben dokumentálva, tesztek ellenőrzik |
| **Licenc probléma**                                | Nem                                            | WYSIWYG könyvtár licencét manuálisan ellenőriztem             |
| **Elavult API javaslat**                           | Igen (verzióeltérés)                           | Saját debug, hivatalos NuGet dokumentáció alapján javítva     |
| **Generált tesztek hiányos lefedés**               | Részben                                        | Tesztek átnézve, hiányzó edge case-ek pótolva                 |
