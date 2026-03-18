# SmartStock Backend (ASP.NET Core + PostgreSQL)

## Stack
- ASP.NET Core Web API (.NET 10)
- Clean Architecture: `Domain` / `Application` (MediatR) / `Infrastructure` (EF Core + PostgreSQL + Dapper) / `WebAPI`
- Auth: ASP.NET Core Identity + JWT (roles: `Admin`, `Sotuvchi`)
- Validation: FluentValidation (MediatR pipeline)
- Mapping: AutoMapper
- Error handling: Global Exception Middleware
- Audit: Product create/price-change logs (`AuditLogs`)

## Run (local)
1. PostgreSQL ishga tushiring (masalan, `localhost:5432`).
2. `src/SmartStock.WebAPI/appsettings.json` dagi `ConnectionStrings:DefaultConnection` ni sozlang.
3. API start:

```bash
dotnet run --project src/SmartStock.WebAPI
```

API start bo‘lganda `EF Core migrations` avtomatik apply bo‘ladi.

## Default seed
`appsettings.json`:
- Admin username: `admin`
- Admin password: `Admin123!`

## Swagger
Startdan keyin Swagger UI:
- `https://localhost:<port>/swagger`

## API Endpoints (asosiy)
- **Auth**
  - `POST /api/auth/login`
  - `POST /api/auth/sellers` (Admin)
- **Products**
  - `GET /api/products`
  - `GET /api/products/{id}`
  - `GET /api/products/by-barcode/{barcode}`
  - `POST /api/products` (Admin)
  - `PUT /api/products/{id}` (Admin)
  - `DELETE /api/products/{id}` (Admin)
- **Stock**
  - `POST /api/stock/in` (Admin, Sotuvchi)
- **POS**
  - `POST /api/pos/sales` (Admin, Sotuvchi)
- **Debt**
  - `POST /api/debts/payments` (Admin, Sotuvchi)
- **Statistics** (Admin)
  - `GET /api/statistics/today-sales`
  - `GET /api/statistics/top-sold?fromUtc=...&toUtc=...`
  - `GET /api/statistics/monthly?year=2026`

## Docker (ixtiyoriy)
Agar Docker o‘rnatilgan bo‘lsa:

```bash
docker compose up -d --build
```

API: `http://localhost:8080/swagger`

