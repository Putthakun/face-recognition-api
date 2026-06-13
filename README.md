<div align="center">

# Face Recognition API

**Core backend for a real-time face-recognition attendance system**

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-512BD4?style=flat)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Azure%20SQL%20Edge-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)](https://hub.docker.com/_/microsoft-azure-sql-edge)
[![Redis](https://img.shields.io/badge/Redis-cache-DC382D?style=flat&logo=redis&logoColor=white)](https://redis.io)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=flat&logo=jsonwebtokens&logoColor=white)](https://jwt.io)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?style=flat&logo=docker&logoColor=white)](https://docker.com)

</div>

---

## Overview

`face-recognition-api` is the **central management and reporting API** for an attendance system built around real-time face recognition. It owns the system of record (employees, cameras, credentials, transactions), issues JWTs for the web dashboard, and bridges the Python face-recognition pipeline with the relational database — keeping a Redis cache of face embeddings in sync so recognition stays fast.

It is one service in a larger system:

```
Edge camera (YOLOv8) → Face server (InsightFace, RabbitMQ) → ── this API ── → SQL Server
                                                                     │
                                                              Vue 3 dashboard
```

- **Edge/recognition services** publish attendance events here (`POST /api/transactions`) after matching a face against the cached embeddings.
- **The web dashboard** authenticates against this API and uses it for all employee, camera, and transaction management.
- **Redis** holds a `face:vectors` hash (EmpId → embedding) that this API loads on startup and keeps updated whenever an employee's photo changes — so the recognition server never has to query SQL directly.

---

## Features

- **JWT authentication** with role-based authorization (`Admin`, `Supervisor`, `Employee`)
- **Employee management** — create/update/delete employees, including photo upload with automatic face-embedding extraction
- **Atomic photo validation** — if no face can be detected in an uploaded photo, the request fails cleanly with no partial database writes
- **Camera management** — full CRUD for registered cameras (location, identifiers)
- **Attendance transactions** — ingest endpoint for the recognition pipeline, plus a paginated, sortable history endpoint for dashboards
- **Face vector cache** — Redis-backed cache of face embeddings, loaded from SQL on startup and kept in sync on every create/update/delete
- **Auto-seeded admin account** — first-run bootstrap creates an initial Admin credential from configuration
- **Dockerized** — multi-stage build producing a slim ASP.NET runtime image

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 Web API |
| ORM / Database | Entity Framework Core 10 + SQL Server (Azure SQL Edge) |
| Auth | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`), BCrypt.Net-Next for password hashing |
| Cache | Redis (StackExchange.Redis) — face embedding cache |
| Architecture | Controller → Service (+ Repository for auth) → EF Core DbContext |
| Container | Docker (multi-stage `sdk:10.0` → `aspnet:10.0`) |

---

## Architecture

```
Controllers/        → HTTP endpoints, [Authorize] role checks
  ├── AuthController       POST /api/auth/login
  ├── EmployeeController    /api/admin/employees   (Admin)
  ├── CameraController       /api/admin/cameras    (Admin)
  ├── RoleController          /api/role           (any authenticated user)
  └── TransactionController   /api/transactions   (ingest + history)

Services/            → business logic
  ├── AuthService            login + JWT issuing
  ├── EmployeeService         CRUD + face embedding extraction + cache sync
  └── CameraService            CRUD

Interfaces/          → service & repository contracts (DI)

Repositories/
  └── UserRepository          credential lookups for auth (only auth uses the repository pattern)

Models/              → EF Core entities (Employee, Credential, Role, EmployeeRole, Camera, FaceEmbedded, Transaction)

Data/                → AppDbContext

Services/FaceRecognitionService → HTTP client to face-recognition-server for embedding extraction
Services/FaceVectorCacheService → Redis "face:vectors" hash sync
```

On startup, the API:
1. Seeds an initial Admin credential if none exists (`AdminSeed:EmpId` / `AdminSeed:Password`).
2. Loads all stored face embeddings from SQL into the Redis `face:vectors` hash, so the recognition server has a warm cache immediately.

---

## API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | Public | Authenticate with EmpId + password, returns JWT |
| `GET` | `/api/role` | Authenticated | List available roles |
| `GET` | `/api/admin/employees` | Admin | List employees |
| `POST` | `/api/admin/employees` | Admin | Create employee (multipart/form-data with photo) |
| `PUT` | `/api/admin/employees/{empId}` | Admin | Partial update (name, role, photo, etc.) |
| `DELETE` | `/api/admin/employees/{empId}` | Admin | Delete employee |
| `GET` | `/api/admin/cameras` | Admin | List cameras |
| `POST` | `/api/admin/cameras` | Admin | Create camera |
| `PUT` | `/api/admin/cameras/{cameraId}` | Admin | Update camera |
| `DELETE` | `/api/admin/cameras/{cameraId}` | Admin | Delete camera |
| `POST` | `/api/transactions` | Internal | Record an attendance event (called by the recognition pipeline) |
| `GET` | `/api/transactions?page=&pageSize=&sortDesc=` | Admin, Supervisor | Paginated attendance history |

Employee create/update return a `400 { message, field: "photo" }` if the uploaded image contains no detectable face — the record is never written in that case.

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (or Azure SQL Edge via Docker)
- Redis
- A running [`face-recognition-server`](https://github.com/Putthakun/face-recognition-server) instance (for face embedding extraction)

### Configuration

Copy `appsettings.json` and set:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FaceRecognitionDb;User Id=sa;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True;"
  },
  "AllowedOrigins": ["http://localhost:5173"],
  "AdminSeed": { "EmpId": "1111", "Password": "YOUR_ADMIN_PASSWORD" },
  "FaceApi": { "BaseUrl": "http://localhost:8001" },
  "Redis": { "ConnectionString": "localhost:6379" },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "face-recognition-api",
    "Audience": "face-recognition-client",
    "ExpiresInHours": 24
  }
}
```

### Run locally

```bash
dotnet restore
dotnet ef database update   # apply migrations
dotnet run
```

### Run with Docker

```bash
docker build -t face-recognition-api .
docker run -p 8080:8080 --env-file .env face-recognition-api
```

> The API expects the shared infrastructure (`face-recognition-infra`) for SQL Server, Redis, and RabbitMQ to be running and reachable.

---

## Related Services

This API is part of a larger system. See [`real-time-face-recognition-attendance-system`](https://github.com/Putthakun/real-time-face-recognition-attendance-system) for the full architecture overview.

| Repo | Role |
|---|---|
| [`face-recognition-edge`](https://github.com/Putthakun/face-recognition-edge) | Captures video, detects faces (YOLOv8), publishes crops to RabbitMQ |
| [`face-recognition-server`](https://github.com/Putthakun/face-recognition-server) | Matches faces against cached embeddings (InsightFace), records transactions via this API |
| [`face-recognition-web`](https://github.com/Putthakun/face-recognition-web) | Vue 3 dashboard for admins/supervisors, consumes this API |
| [`face-recognition-infra`](https://github.com/Putthakun/face-recognition-infrastructure) | Shared SQL Server, Redis, RabbitMQ via Docker Compose |
