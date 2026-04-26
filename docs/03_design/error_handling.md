# Error Handling

## Áttekintés

A rendszer egységes hibakezelést alkalmaz backend és frontend szinten egyaránt.
A backend minden választ `ResponseResult<T>` objektumba csomagol, a frontend
egy globális HTTP interceptoron keresztül dolgozza fel a hibákat.

---

## 1. Egységes error object (backend)

### ResponseResult\<T\>

Minden API endpoint `ResponseResult<T>` típusú választ ad vissza:

```json
// Sikeres válasz
{
  "isSuccess": true,
  "value": { ... },
  "errorField": null,
  "message": null
}

// Általános hiba
{
  "isSuccess": false,
  "value": null,
  "errorField": null,
  "message": "Nem található az erőforrás"
}

// Field-specifikus validációs hiba
{
  "isSuccess": false,
  "value": null,
  "errorField": "email",
  "message": "Ez az email cím már foglalt"
}
```

### C# forrás

```csharp
// Services.Types namespace
public class ResponseResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? ErrorField { get; private init; }
    public string? Message { get; private init; }

    public static ResponseResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static ResponseResult<T> Failure(string message) =>
        new() { IsSuccess = false, Message = message };

    public static ResponseResult<T> Failure(string field, string message) =>
        new() { IsSuccess = false, ErrorField = field, Message = message };
}
```

---

## 2. Hibakategóriák

| HTTP kód | Kategória | Mikor keletkezik | Példa message |
|----------|-----------|-----------------|---------------|
| 400 | Validációs hiba | DTO constraint megsértése, üzleti logika hiba | `"Az email cím már foglalt"` |
| 401 | Auth hiba | Hiányzó vagy lejárt JWT token | `"Bejelentkezés szükséges"` |
| 403 | Jogosultság hiba | Nincs megfelelő szerepkör | `"Nincs jogosultságod ehhez"` |
| 404 | Not found | Erőforrás nem létezik | `"Resource not found"` |
| 409 | Konfliktus | Duplikált adat, ütköző művelet | `"Már létezik ilyen rekord"` |
| 500 | Belső hiba | Kezeletlen kivétel, DB hiba | `"Internal server error"` |

---

## 3. Validációs rétegek

A rendszer kétszintű validációt alkalmaz:

### 3.1 Frontend (első vonal)
- Angular reaktív form validátorok (required, minLength, pattern stb.)
- A request el sem indul, ha a form invalid
- Azonnali, user-facing visszajelzés

### 3.2 Backend DTO validáció (második vonal)
- Data Annotation attribútumok (`[Required]`, `[MaxLength]`, `[Range]` stb.)
- A Swagger/OpenAPI automatikusan dokumentálja ezeket a constrainteket
- Ha a frontend validáció megkerülhető (pl. direkt API hívás), a backend is véd

---

## 4. Frontend error mapping (Angular interceptor)

A globális `errorInterceptor` egységesen dolgozza fel a HTTP hibákat:

```typescript
export interface HttpValidationError {
  title: string;
  fieldErrors: { [key: string]: string[] };
}
```

### Mapping logika

| HTTP státusz   | Feldolgozás                                           | Eredmény                            |
|----------------|-------------------------------------------------------|-------------------------------------|
| 400            | `parseValidationErrors()` – field + message kinyerése | `fieldErrors[field] = [message]`    |
| 401            | `parseValidationErrors()` – auth hiba üzenet          | `title: "Bejelentkezés szükséges"`  |
| 404            | Statikus válasz                                       | `title: "Resource not found"`       |
| 500            | Statikus válasz                                       | `title: "Internal server error"`    |
| Egyéb          | Dinamikus                                             | `title: "Server returned code {N}"` |

### Field-specifikus hiba feldolgozása

A `parseValidationErrors()` két forrásból nyeri ki a field hibákat:

1. **ResponseResult field hiba** (`errorField` + `message`):
```json
{ "errorField": "email", "message": "Már foglalt" }
→ fieldErrors: { "email": ["Már foglalt"] }
```

2. **ASP.NET ModelState hiba** (`errors` objektum):
```json
{ "errors": { "Summary.Name": ["Kötelező mező"] } }
→ fieldErrors: { "name": ["Kötelező mező"] }
```
> A `Summary.` prefix automatikusan le van vágva a kulcsokról.

---

## 5. User-facing üzenetek elvei

- **Érthetőek:** nem technikai stack trace, hanem emberi üzenet
- **Cselekvésre ösztönöznek:** pl. *"Próbáld újra"*, *"Ellenőrizd az email címet"*
- **Specifikusak ahol lehet:** field-szintű hiba jelzi, melyik mezőt kell javítani
- **PII-mentes:** logba és hibaüzenetbe nem kerül jelszó, token, személyes adat

### Példák

| Szituáció     | Rossz üzenet  ❌                | Jó üzenet ✅                              |
|---------------|--------------------------------|--------------------------------------------|
| Foglalt email | `UNIQUE constraint failed`     | `Ez az email cím már foglalt`              |
| Lejárt token  | `401 Unauthorized`             | `A munkamenet lejárt, jelentkezz be újra`  |
| Nem található | `null reference`               | `A keresett tartalom nem található`        |
| Szerverhiba   | `NullReferenceException at...` | `Váratlan hiba történt, próbáld újra`      |

---

## 6. Retry stratégia

| Hibatípus       | Retry?     | Indok                                                    |
|-----------------|------------|----------------------------------------------------------|
| 400 Validáció   | ❌ Nem      | A user javítása szükséges, automatikus retry értelmetlen |
| 401 Auth        | ❌ Nem      | Token megújítás vagy újbóli bejelentkezés kell           |
| 403 Jogosultság | ❌ Nem      | Jogosultság nem változik retry-ra                        |
| 404 Not found   | ❌ Nem      | Az erőforrás nem létezik                                 |
| 408 / Timeout   | ✅ Igen     | Hálózati probléma, 1-2 retry indokolt                    |
| 500 Szerverhiba | ⚠️ Egyszer | Tranziens hiba lehet, 1 retry után feladd                |
| Network error   | ✅ Igen     | Kapcsolódási hiba, exponential backoff ajánlott          |

---

## 7. Logolás

### Mi kerül logba
- HTTP státuszkód és endpoint path
- Hiba típusa és üzenete
- Kérés időbélyege
- Correlation ID (ha van)

### Mi NEM kerül logba (PII védelem)
- Jelszó, password hash
- JWT token tartalom
- Email cím, teljes név
- Személyes azonosítók (születési dátum stb.)

### Hiba megtalálása
- A szerver logban az endpoint path + státuszkód + időbélyeg alapján szűrhető
- 500-as hibáknál a stack trace csak szerver oldalon látható, kliensnek nem kerül ki