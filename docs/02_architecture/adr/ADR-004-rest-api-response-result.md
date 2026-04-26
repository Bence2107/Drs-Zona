# ADR-004: REST API design + egységes ResponseResult hibamodell

- **Dátum:** 2026-02-01
- **Státusz:** Accepted

---

## Context

Az API-nak konzisztens kommunikációs kontraktust kell biztosítania a frontend felé.
A fejlesztés során AI-asszisztált kódgenerálás is történt, ahol az inkonzisztens
hibamodell (néha string, néha objektum visszaadása) komoly problémát okozhat.
Az Angular frontend ng-openapi-gen-nel generálja a típusos klienst a Swagger sémából, így natívan ki lehet építeni a frontenden
egy service layert a metódusok meghívásával.

Kényszerek:
- Minden endpoint azonos formátumú választ adjon (siker és hiba esetén egyaránt)
- A frontend interceptor egységesen tudja kezelni a hibákat
- Swagger/OpenAPI-ból generálható legyen a kliens kód

---

## Decision

REST API-t alkalmazunk `/api/v1/` prefix-szel; minden válasz `ResponseResult<T>`
generikus objektumba van csomagolva, amely `IsSuccess`, `Value`, `ErrorField`
és `Message` mezőket tartalmaz.

---

## Alternatívák

**A) GraphQL**
- Rugalmas lekérdezések, kliens határozza meg a mezőket
- Nagy tanulási görbe, .NET tooling komplexebb
- Swagger/OpenAPI generálás nem alkalmazható
- Over-fetching/under-fetching problémák nem relevánsak ebben a scope-ban

**B) Sima HTTP státuszkód + string hibaüzenet**
- Egyszerű implementáció
- Frontend nem tudja egységesen feldolgozni (string vs. objektum keveredés)
- Típusbiztos kliens generálás nehéz
- Ha változik az API felépítése, manuálisan kell azt a frontend-nél is módosítani.

**C) Problem Details (RFC 7807)**
- Szabványos hibaformátum
- Jól dokumentált, ismert struktúra
- A `ResponseResult<T>` a siker és hiba esetet egységesen kezeli, RFC 7807 csak hibára vonatkozik

---

## Következmények

**Pozitív:**
- Frontend mindig tudja, mit vár: `isSuccess` flag alapján elágazik
- `errorField` mező lehetővé teszi field-szintű validációs hibaüzenetet
- Swagger automatikusan dokumentálja a `ResponseResult<T>` sémát minden endpointnál
- ng-openapi-gen típusbiztos klienst generál belőle

**Negatív / kockázat:**
- Siker esetén is van wrapper overhead (`{ isSuccess: true, value: {...} }`)
- HTTP státuszkódok és `isSuccess` flag redundánsan kódolják ugyanazt az információt
- Generikus típus Swaggerben néha nehezebben olvasható sémát generál

---

## Verification

- Angular `errorInterceptor` egységesen kezeli az összes hibát (`isSuccess: false`)
- 69 controller teszt ellenőrzi a helyes HTTP státuszkód + ResponseResult kombinációt
- `GET /swagger/v1/swagger.json` – séma exportálható, ng-openapi-gen lefut hibamentesen
