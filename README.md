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

## 🚀 Roadmap

### v1.0.0 — Initial Release *(Planned: Q2 2026)*

* 📰 Public **News & Articles** listing
* 💬 Comment system under articles
* 🧑‍💻 Authentication & user profiles
* ✍️ Article creation & editing (authorized users)
* 🏆 Race results stored in internal database
* 🗺️ Grand Prix & race track information pages
* 📊 Interactive polls (rules, results, predictions)
* 🗂️ Category-based browsing (Articles, Polls, etc.)

---

## ✨ Core Features

### 📰 News & Articles

* List all published motorsport news
* Detailed article pages with comments
* Category-based filtering

### 🔐 Authentication System

* User registration & login
* Profile management
* Role-based permissions (e.g. Admin, Author, User)

### 🏁 Results & Statistics

* Race results stored and served from internal SQL database
* Expandable structure for multiple racing series

### 🌍 Grand Prix & Track Database

* Dedicated pages for each Grand Prix
* Track layouts, stats, and historical data

### 📊 Polls & Community Interaction

* Fan polls about rule changes, race outcomes, and more
* Real-time results visualization

---

## 🛠️ Tech Stack

### Backend

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-purple)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-blue)
![SQL](https://img.shields.io/badge/Database-SQL-lightgrey)

* ASP.NET Core Web API
* Entity Framework Core (Code First)
* SQL-based relational database (Which provides with EF Core: https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)

### Frontend

![Angular](https://img.shields.io/badge/Angular-Framework-red)
![TypeScript](https://img.shields.io/badge/TypeScript-Language-blue)
![Material](https://img.shields.io/badge/UI-Angular%20Material-orange)

* Angular
* TypeScript
* Angular Material or custom UI components

---

## 📁 Project Structure

```text
Drs-Zóna/
│
├── client/        # Angular frontend
│
├── server/        # ASP.NET Core backend
│
└── README.md
```

---

## ⚙️ Getting Started

### Prerequisites

* .NET SDK (latest LTS)
* Node.js (LTS recommended)
* npm
* SQL Server / compatible SQL database

### Backend Setup

```bash
cd server
dotnet run
```

The API will start on the configured local port.

### Frontend Setup

```bash
cd client
npm install
ng serve
```

The Angular app will be available at:

```
http://localhost:4200
```

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2026 Drs-Zóna

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🤝 Contributing *(Planned)*

Contribution guidelines, issue templates, and pull request workflows will be added as the project approaches its first release.

---

## 🏎️ Vision

**Drs-Zóna** aims to become a central hub for motorsport enthusiasts — combining reliable data, quality content, and an engaged community.

*Stay fast. Stay informed.* 🏁
