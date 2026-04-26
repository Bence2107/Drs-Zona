# Observability

## 1. Logging

### Log szintek

| Szint         | Mikor                                | Példa                                         |
|---------------|--------------------------------------|-----------------------------------------------|
| `Information` | Normál működés, kérések              | `GET /api/v1/drivers 200 45ms`                |
| `Warning`     | Nem kritikus probléma                | `JWT token lejárt, felhasználó kijelentkezve` |
| `Error`       | Kezelt hiba, service szintű probléma | `Driver not found: id=abc-123`                |
| `Critical`    | Rendszer szintű hiba                 | `DB kapcsolat nem elérhető`                   |


> Az EF Core log szintje `Warning`-ra állítva – különben minden SQL query megjelenik a logban.

### Amit NEM logolunk (PII védelem)

- Jelszó, `password_hash`
- JWT token tartalom
- Email cím, teljes név
- `current_session_id`


### Logok helye

- **Lokál fejlesztés:** konzol output (`dotnet run` terminál)
- **Szűrés:** `dotnet run 2>&1 | grep "Error\|Warning\|fail"`

---

## 2. Healthcheck

### Endpoint

```
GET http://localhost:5244/health
```

### Mit vizsgál

| Check        | Mit ellenőriz                           |
|--------------|-----------------------------------------|
| `postgresql` | DB kapcsolat él-e, válaszol-e a szerver |

---

## 3. Javasolt metrikák

| Metrika                             | Típus     | Miért fontos                           |
|-------------------------------------|-----------|----------------------------------------|
| **API válaszidő (p95 latency)**     | Histogram | Teljesítmény regresszió korai jelzése  |
| **HTTP error rate (4xx/5xx arány)** | Counter   | Hibás kérések arányának trendje        |
| **Aktív DB kapcsolatok száma**      | Gauge     | Connection pool kimerülés előrejelzése |
| **Bejelentkezési hibák száma**      | Counter   | Brute force támadás detektálása        |
| **Request throughput (req/sec)**    | Counter   | Terhelés monitorozása                  |

### Mérés módja (lokál, eszköz nélkül)

```bash
# Válaszidő mérése
curl -o /dev/null -s -w "%{time_total}s\n" http://localhost:5244/api/v1/drivers

# Health ellenőrzés
curl http://localhost:5244/health
```

---

## 4. Tracing (tervezett)

Jelenleg nincs distributed tracing. Jövőbeli bevezetési terv:

- **Request ID:** minden kéréshez egyedi `X-Request-Id` header generálása middleware-ben
- **Structured logging:** Serilog + JSON formátum, ahol minden log bejegyzés tartalmazza a request ID-t
- **Eszköz:** OpenTelemetry .NET SDK (ingyenes, vendor-agnosztikus)

---

## 5. Debugging guide

### Hol találom a logokat?

```bash
# Összes log
dotnet run

# Csak hibák és warningok
dotnet run 2>&1 | grep -E "error|warn|fail|exception" -i

# EF Core SQL lekérdezések (debug célra)
# appsettings.Development.json-ban:
# "Microsoft.EntityFrameworkCore": "Information"
```

### Mit keressek hiba esetén?

| Probléma    | Mit keressek a logban                 |
|-------------|---------------------------------------|
| 500-as hiba | `Exception`, `Unhandled`, `Critical`  |
| DB hiba     | `Npgsql`, `connection`, `timeout`     |
| Auth hiba   | `401`, `JWT`, `token`                 |
| Lassú kérés | `Request finished` sorok – ms értékek |

### Tipikus debug folyamat

```
1. curl http://localhost:5000/health → DB él-e?
2. dotnet run terminál → van-e exception?
3. Érintett endpoint közvetlen hívása curl-lel
4. Ha DB hiba: psql kapcsolat tesztelése
5. Ha kód hiba: stack trace azonosítása → érintett service megtalálása
```

---

## 6. SLO gondolkodás (lokál környezetre)

| Célkitűzés              | Érték   | Mérés                          |
|-------------------------|---------|--------------------------------|
| API válaszidő (p95)     | < 500ms | `curl` időmérés                |
| Healthcheck válasz      | < 100ms | `curl /health`                 |
| Tesztok pass rate       | 100%    | `dotnet test`                  |
| Build sikerességi arány | 100%    | `dotnet build` 0 warning/error |