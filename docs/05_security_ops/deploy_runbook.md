# Deploy Runbook

## 1. Környezeti modell

| Környezet        | Leírás                                                   | URL                                                                  |
|------------------|----------------------------------------------------------|----------------------------------------------------------------------|
| **Lokál (dev)**  | Fejlesztői gépen futó egyetlen környezet                 | `http://localhost:5000` (API), `http://localhost:4200` (Frontend)    |
| **Külső forrás** | Külső eszköre kitelepített build CloudFlare-en keresztül | `https://drs-zona.hu/`                                               |


Jelenleg csak lokális környezet létezik. Staging és production környezet nem konfigurált.

---

## 2. Előfeltételek

| Eszköz      | Verzió | Ellenőrzés         |
|-------------|--------|--------------------|
| .NET SDK    | 9.x    | `dotnet --version` |
| Node.js     | 20.19+ | `node --version`   |
| pnpm        | 10+    | `pnpm --version`   |
| Docker      | –      | `docker --version` (lokális adatbázishoz; helyette saját PostgreSQL 15+ is jó) |
| Angular CLI | 17+    | `ng version`       |

---

## 3. Deploy lépések (lokál)

### 3.1 Első indítás (fresh install)

```bash
# 1. Repository klónozása
git clone <repo-url>
cd <projekt-mappa>

# 2. Backend függőségek visszaállítása
cd server
dotnet restore

# 3. Környezeti változók beállítása (a repository gyökerében)
cd ..
cp .env.example .env
# Töltsd ki a .env-et

# 4. Adatbázis indítása Dockerben, majd a migrációk futtatása (részletek: 4. fejezet)
docker compose up -d
cd server/Context
dotnet ef database update

# 5. Backend indítása
cd ../API
dotnet run

# 6. Frontend függőségek (új terminál, a repository gyökeréből)
cd client
pnpm install

# 7. Frontend indítása
ng serve
```

### 3.2 Napi indítás (már beállított környezet)

```bash
# Adatbázis (ha még nem fut)
docker compose up -d

# Terminal 1 – Backend
cd server && dotnet run --launch-profile https

# Terminal 2 – Frontend
cd client && pnpm start
```

**Portok:** alapból 4200 (frontend), 5244 (API HTTP) és 7221 (API HTTPS). Más portokhoz állítsd be a `FRONTEND_PORT`, `API_PORT` és `API_HTTPS_PORT` változókat a `.env`-ben; az API, a dev proxy és a `pnpm open-api-gen` is ezeket követi. Élesben ezeket a rendszer figyelmen kívül hagyja, ott a port az `ASPNETCORE_URLS`-ből jön.

### 3.3 Production (systemd)

Az éles szerveren egy systemd service futtatja a `server/API/publish/API.dll`-t `Production` környezetben. Ugyanaz a folyamat szolgálja ki az API-t és az Angular buildet (`client/dist/drs-zona/browser`).

**Egyszeri átállás** (a `.env` a repository gyökerébe kerül):

```bash
# 1. A titkos adatok áthelyezése
mv /home/tvbox/Drs-Zona/server/API/.env /home/tvbox/Drs-Zona/.env

# 2. A régi build által bemásolt példányok törlése (különben ezek töltődnének be)
rm -f /home/tvbox/Drs-Zona/server/API/bin/Debug/net9.0/.env /home/tvbox/Drs-Zona/server/API/publish/.env
```

A unit fájlban:

```ini
WorkingDirectory=/home/tvbox/Drs-Zona/server/API/publish
Environment=ENV_FILE=/home/tvbox/Drs-Zona/.env
```

**Deploy:**

```bash
cd /home/tvbox/Drs-Zona
dotnet publish server/API -c Release -o server/API/publish
cd client && pnpm install --frozen-lockfile && pnpm build
sudo systemctl daemon-reload && sudo systemctl restart <service>
```

**Ellenőrzés:** a `journalctl -u <service>` első sora megmutatja, melyik `.env` töltődött be (`Loaded environment from …`).

**Feltöltések:** alapból a `server/API/publish/uploads` mappába kerülnek. Ha a build kimenetén kívül szeretnéd tartani őket, állítsd be az `UPLOADS_PATH`-t a `.env`-ben, és másold át a meglévő fájlokat az új helyre.

### 3.4 API változás után

```bash
# 1. Backend újraindítása
# 2. API kliens újragenerálása
cd client
pnpm open-api-gen
ng serve
```

### 3.5 Lokális adatbázis Dockerben

Csak fejlesztéshez: a `docker-compose.yml` egy PostgreSQL 17-et indít, az API továbbra is `dotnet run`-nal fut, és `localhost`-on éri el. Az adatbázis ugyanazokból a `DB_*` változókból jön létre, amiket az API olvas, ezért a kapcsolat külön beállítás nélkül működik. Docker Desktopban `drs-zona-backend` néven jelenik meg, a konténer neve `drs-zona-backend-db`. Az adatok a `drs-zona-backend_db-data` volume-ban maradnak meg.

```bash
docker compose up -d        # indítás (a repository gyökeréből)
docker compose ps           # fut-e, healthy-e
docker compose logs -f db   # naplók
docker compose down         # leállítás, az adatok megmaradnak
docker compose down -v      # leállítás ÉS az adatbázis törlése
```

- **Foglalt 5432-es port** (ha már fut egy helyi PostgreSQL): állítsd a `DB_PORT`-ot pl. `5433`-ra a `.env`-ben. A Docker ezen a porton teszi elérhetővé az adatbázist, és az API is ezt használja.
- **A `DB_NAME`, `DB_USER` és `DB_PASSWORD` csak az első indításkor számít.** Ha később módosítod őket, `docker compose down -v` után jön létre újra az adatbázis, ami az adatokat is törli.
- **Más helyen lévő `.env`:** `docker compose --env-file <útvonal> up -d`
- **Meglévő helyi adatok átmásolása** (helyi PostgreSQL az 5432-es, Docker az 5433-as porton):

  ```bash
  pg_dump -h localhost -p 5432 -U postgres -Fc drs_zona > drs_zona.dump
  pg_restore -h localhost -p 5433 -U postgres -d drs_zona --no-owner drs_zona.dump
  ```

---


## 4. Migrációk

A migrációk a `server/Context/Migrations` mappában vannak, és a gépedről futnak, nem a konténerből. A `dotnet ef` ugyanazt a `.env`-et olvassa, mint az API (`DB_HOST`, `DB_PORT`…), ezért a Dockeres adatbázist is külön beállítás nélkül éri el.

**Előfeltételek (egyszer):**

```bash
dotnet tool install --global dotnet-ef --version 9.*
```

**Futtatás a Dockeres adatbázison:**

```bash
# 1. Adatbázis indítása (a repository gyökeréből)
docker compose up -d
docker compose ps            # drs-zona-backend-db: "healthy"

# 2. Migrációk futtatása
cd server/Context
dotnet ef database update    # az első sor: Loaded environment from …/.env

# 3. Ellenőrzés: létrejöttek-e a táblák (a saját DB_USER / DB_NAME értékeiddel)
docker compose exec db psql -U postgres -d drs_zona -c '\dt'
```

**Egyéb parancsok** (mind a `server/Context` mappából):

```bash
# Összes migráció listázása (melyik van már lefuttatva)
dotnet ef migrations list

# Új migráció létrehozása modellváltozás után
dotnet ef migrations add <MigrationName>

# Visszagörgetés egy korábbi migrációra
dotnet ef database update <PreviousMigrationName>

# Séma exportálása
dotnet ef dbcontext script -o ../../docs/03_design/schema.sql
```

**Tiszta adatbázis** (minden adat törlődik):

```bash
docker compose down -v && docker compose up -d
cd server/Context && dotnet ef database update
```

**Éles szerveren:** ugyanígy, a szerveren a `server/Context` mappából, a `dotnet-ef` tool telepítése után. A parancs a repository gyökerében lévő `.env`-et találja meg, vagy add meg előtte az útvonalat: `ENV_FILE=/home/tvbox/Drs-Zona/.env dotnet ef database update`. Futtatás előtt készíts mentést (`pg_dump`).

**Gyakori hibák:**

- `Missing environment variables: …`: a `dotnet ef` nem találta a `.env`-et. Az első kimeneti sor mutatja, melyik fájlt olvasta.
- Kapcsolódási hiba: nem fut a konténer (`docker compose ps`), vagy a `DB_PORT` nem egyezik a `.env` és a futó konténer között. `DB_PORT` változás után: `docker compose up -d`.

---

## 5. Konfiguráció és titkos adatok

A rendszer a környezeti változókat a repository gyökerében található `.env` fájlból olvassa be. Az összes változót, azok kötelező voltát és alapértékét a [`.env.example`](../../.env.example) tartalmazza.

```bash
cp .env.example .env
```

A `.env` keresési sorrendje (az API, a `dotnet ef` és a tesztek ugyanígy keresik):

1. az `ENV_FILE` változóban megadott útvonal (pl. `~/.config/drs-zona/.env`)
2. a legközelebbi `.env` az indítási mappától felfelé, a repository gyökeréig
3. `server/API/.env` (a régi hely)

A már beállított környezeti változók (systemd, docker compose, shell) felülírják a fájl értékeit. Hiányzó kötelező változó esetén az API el sem indul, és egy hibaüzenetben felsorolja az összeset.

> [!WARNING]
> A .env fájlt soha ne commitold a verziókövetőbe! Ellenőrizd, hogy a .gitignore fájl tartalmazza-e a bejegyzést.

---

## 6. Rollback

Ha a deploy (vagy migráció) után valami nem működik:

```bash
# 1. Visszagörgetés az előző migrációra
dotnet ef database update <PreviousMigrationName> --context Context.EfContext

# 2. Kód visszaállítása
git checkout <előző-tag-vagy-commit>

# 3. Újraindítás
dotnet run
```

---

## 7. Verziózás

```bash
# Release tag létrehozása
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0

# Aktuális verzió lekérdezése
git describe --tags
```

Konvenció: `v<major>.<minor>.<patch>` – szemantikus verziózás.

---

## 8. Incident forgatókönyvek

### Incident #1 – API nem válaszol / 500-as hibák

**Tünetek:**
- Frontend `Internal server error` üzenetet jelenít meg
- Az Angular interceptor `500` státuszt kap
- A `/health` endpoint nem válaszol vagy 500-at ad

**Gyors diagnózis:**
```bash
# 1. Fut-e a backend?
curl http://localhost:5244/health

# 2. Konzol output ellenőrzése – van-e exception?
# (dotnet run terminál outputja)

# 3. DB elérhető-e?
psql -U <user> -d <dbname> -c "SELECT 1"
```

**Ideiglenes mitigáció:**
```bash
# Backend újraindítása
Ctrl+C
dotnet run
```

**Végleges javítás iránya:**
- Ha DB hiba: kapcsolat string ellenőrzése, PostgreSQL service újraindítása
- Ha kód hiba: stack trace azonosítása a konzol outputból, érintett service javítása
- Ticket: "500 hiba azonosítása és javítása – [exception típus]"

---

### Incident #2 – Felhasználók nem tudnak bejelentkezni

**Tünetek:**
- Login form elküldés után `401 Unauthorized` vagy `400` válasz
- Az Angular interceptor `"Validation error occurred"` üzenetet ad
- Más endpointok működnek

**Gyors diagnózis:**
```bash
# 1. Health endpoint ellenőrzése
curl http://localhost:5244/health

# 2. Direkt API hívás tesztelése
curl -X POST http://localhost:5244/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"test"}'

# 3. JWT konfiguráció ellenőrzése
# .env – JWT_SECRET megvan-e, és legalább 32 karakter-e?
# A log első sora mutatja, melyik .env töltődött be (Loaded environment from …)
```

**Ideiglenes mitigáció:**
- JWT secret újragenerálása és beállítása ha hiányzik
- Backend újraindítása konfiguráció változás után

**Végleges javítás iránya:**
- Ha JWT secret hiányzik vagy megváltozott: a `JWT_SECRET` javítása a `.env`-ben (élesben az `ENV_FILE` által megadott fájlban), majd újraindítás. A secret cseréje minden kiadott tokent érvénytelenít.
- Ha DB probléma: `users` tábla elérhetőségének ellenőrzése
- Ticket: "Auth hiba kivizsgálása – [401/400 részletek]"

---

### Incident #3 – Adatbázis kapcsolat megszakad

**Tünetek:**
- Minden API endpoint hibát ad vissza
- Konzolban `Npgsql.NpgsqlException` vagy `connection refused` üzenet
- `/health` endpoint DB check sikertelen

**Gyors diagnózis:**
```bash
# 1. PostgreSQL service fut-e?
pg_ctl status
# vagy
sudo systemctl status postgresql

# 2. Kapcsolódás tesztelése
psql -U <user> -d <dbname> -c "SELECT 1"
```

**Ideiglenes mitigáció:**
```bash
# PostgreSQL újraindítása
sudo systemctl restart postgresql
# majd backend újraindítása
dotnet run
```

**Végleges javítás iránya:**
- Connection string ellenőrzése ha service fut de nem kapcsolódik
- Ticket: "DB kapcsolat hiba – PostgreSQL log ellenőrzése"