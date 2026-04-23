<div align="center">

<img width="300" height="113" alt="Drs Zóna" src="https://github.com/user-attachments/assets/8155e5b8-abd8-4821-800c-564b758500b2" />

# 🏁 Drs-Zóna — The Motorsport Hub

*A modern, community-driven motorsport portal bringing fans, events, and data together in one place.*

![Status](https://img.shields.io/badge/status-planned-orange)
![Version](https://img.shields.io/badge/version-v0.5.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Tech](https://img.shields.io/badge/.NET-ASP.NET%20Core-purple)
![Frontend](https://img.shields.io/badge/frontend-Angular-red)

</div>

---

## 📌 Overview

**Drs-Zóna** is a web-based motorsport hub designed for fans who want everything in one place: breaking news, in-depth articles, race results, Grand Prix information, and interactive polls.

The goal is to create a scalable, modern platform with a strong backend and a clean, fast frontend experience.

---

## ✨ Core Features

### 📰 News & Articles

* List all published motorsport news
* Detailed article pages with comments
* Category-based filtering

### 🔐 Authentication System

* User registration & login
* Profile management
* Role-based permissions (Admin, Author, User)

### 🏁 Results & Statistics

* Race results stored in SQL database
* Expandable structure for multiple racing series

### 🌍 Grand Prix & Track Database

* Dedicated pages for each Grand Prix
* Track layouts, stats, and historical data

### 📊 Polls & Community Interaction

* Fan polls about rules, races, predictions
* Real-time result visualization

---

## 🛠️ Tech Stack

### Backend

* ASP.NET Core Web API
* Entity Framework Core (Code First)
* PostgreSQL

### Frontend

* Angular
* TypeScript
* Angular Material

---

## 📁 Project Structure

```text
Drs-Zóna/
│
├── client/        # Angular frontend
├── server/        # ASP.NET Core backend
├── docs/          # Documentation (architecture, design, etc.)
└── README.md
```

---

## ⚙️ Getting Started (Quickstart)

### 📋 Prerequisites

* .NET SDK (LTS)
* Node.js (LTS)
* npm
* PostgreSQL

---

### 🔧 1. Clone the repository

```bash
git clone https://github.com/your-repo/drs-zona.git
cd drs-zona
```

---

### ⚙️ 2. Configure environment variables

Create `.env` files in server/API based should look like this:

```bash
# =========================
# Database Configuration
# =========================
DB_HOST=localhost
DB_PORT=5432
DB_NAME=drs_zona
DB_USER=postgres
DB_PASSWORD=your_password_here

# =========================
# JWT Configuration
# =========================
JWT_SECRET=your_super_secret_key_here
JWT_ISSUER=drs-zona-api
JWT_AUDIENCE=drs-zona-client
JWT_EXPIRATION_MINUTES=60

# =========================
# CORS Configuration
# =========================
CORS_ORIGINS=http://localhost:4200
```

> ⚠️ Note: Secrets (API keys, passwords) are **not included** in the repository.

---

### 🗄️ 3. Database setup

* Ensure PostgreSQL is running
* Apply migrations:

```bash
cd server/Context
dotnet ef database update
```

---

### ▶️ 4. Run backend

```bash
cd server
dotnet run --launch-profile https
```

Expected result:

* API starts successfully
* Example: `http://localhost:7221`
* Health endpoint available: `/health`
* The Swagger's route: *localhost*/swagger/index.html

---

### 💻 5. Run frontend

```bash
cd client
npm install
npm start
```

Application will be available at:

```
http://localhost:4200
```

---

### ✅ 6. Verify system

After startup:

* Frontend loads without errors
* API responds (e.g. `/api/Article`)
* Database connection works

---

## 🧪 Running Tests

### Backend tests

```bash
cd server
dotnet test
```

### Frontend tests

```bash
cd client
npm test
```

> Test results should show all tests passing.

---

## 📊 Evidence & Quality

This project follows an **evidence-first approach**:

* Automated tests (unit, integration, e2e)
* API responses verifiable via endpoints
* UI flows demonstrated via screenshots (see `/docs/ux`)
* Logs and health checks available

---


> ### 🔐 Security Note

All sensitive configuration (database credentials, JWT secrets) is handled via environment variables.

No secrets are stored in the repository.


---

## 🚀 Roadmap

### v1.0.0 — Initial Release *(Planned: Q2 2026)*

* 📰 News & Articles
* 💬 Comment system
* 🔐 Authentication & profiles
* ✍️ Article management
* 🏆 Race results
* 🌍 Grand Prix database
* 📊 Polls
* 🗂️ Category filtering

---

## 📄 License

This project is licensed under the **MIT License**.

---

## 🤝 Contributing *(Planned)*

Contribution guidelines will be added in future versions.

---

## 🏎️ Vision

**Drs-Zóna** aims to become a central hub for motorsport enthusiasts — combining reliable data, quality content, and an engaged community.

*Stay fast. Stay informed.* 🏁
