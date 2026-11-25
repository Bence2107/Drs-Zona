<div align="center">

<img width="300" height="113" alt="Drs Zóna" src="https://github.com/user-attachments/assets/8155e5b8-abd8-4821-800c-564b758500b2" />

# Drs-Zóna

A community motorsport portal for news, results and fan interaction.

![.NET](https://img.shields.io/badge/.NET-9-512BD4)
![Angular](https://img.shields.io/badge/Angular-21-DD0031)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)

</div>

## Overview

Drs-Zóna brings motorsport news, race results and community features into one place. Journalists write articles, editors maintain championship data, and fans read, comment and vote.

- News and reviews with threaded comments
- Results and standings for multiple series, with Grand Prix and circuit data
- Polls
- User accounts with User, Author, Editor and Admin roles
- Article editor for authors, data management for editors

## Tech stack

| Layer | Technology |
| --- | --- |
| Frontend | Angular 21, Angular Material, TypeScript |
| Backend | ASP.NET Core 9 Web API, Entity Framework Core |
| Database | PostgreSQL |
| Tooling | pnpm, Docker Compose (local database), xUnit, Vitest |

## Repository structure

```text
Drs-Zona/
├── client/                    Angular frontend
│   ├── public/                Static images and favicon
│   ├── scripts/               Dev server, .env loading, API client generation
│   └── src/app/
│       ├── api/               Generated API client (do not edit)
│       ├── components/        Shared UI: header, dialogs, lists
│       ├── directives/        Form directives
│       ├── guards/            Route guards by role
│       ├── interceptors/      Adds the JWT to requests
│       ├── models/            UI-only models
│       ├── pages/             Routed pages, admin pages under pages/admin
│       ├── pipes/             Display pipes
│       ├── services/          API services and error handling
│       └── shared/            Shared helpers
├── server/                    ASP.NET Core API (Drs-Zona.sln)
│   ├── API/                   Entry point, controllers, Program.cs
│   ├── Context/               EfContext, migrations, configuration loading
│   ├── DTOs/                  Request and response contracts
│   ├── Entities/              EF Core entity models
│   ├── Repositories/          Data access, interfaces and implementations
│   ├── Services/              Business logic, image handling, auth
│   └── Tests/                 Unit, integration and controller tests
├── docker-compose.yml         Local PostgreSQL for development
└── .env.example               Template for all configuration
```

Each part has its own README: [client](client/README.md) and [server](server/README.md).

## Quickstart

Requires the .NET 9 SDK, Node.js 20.19+, pnpm 12+, Docker and the `dotnet-ef` tool.

```bash
git clone https://github.com/Bence2107/Drs-Zona.git
cd Drs-Zona
cp .env.example .env              # fill in DB_PASSWORD and JWT_SECRET

docker compose up -d              # start PostgreSQL
cd server/Context && dotnet ef database update && cd ../..

cd server/API && dotnet run --launch-profile https
```

In a second terminal:

```bash
cd client
pnpm install
pnpm start
```

Open `http://localhost:4200`. The API's Swagger UI is at `http://localhost:5244/swagger`.

See the [server README](server/README.md) and the [client README](client/README.md) for configuration, the local database and tests.

## Vision

Drs-Zóna aims to become a central hub for motorsport fans, combining reliable data, quality content and an engaged community.

*Stay fast. Stay informed.*
