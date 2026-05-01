[README.md](https://github.com/user-attachments/files/27286347/README.md)
# Restaurant System API

Restaurant System is an ASP.NET Core 8 Web API for restaurant operations such as authentication, users, roles, customers, categories, products, tables, orders, payments, refunds, and reporting.

The solution is organized as a layered backend:

- `API_Layer` exposes the HTTP API, Swagger, JWT auth, authorization policies, and rate limiting.
- `BusinessLogicLayer` contains application services, validation flow, MediatR handlers, and mapping logic.
- `DataAccessLayer` handles SQL Server access through repositories and database settings.
- `Contracts` contains shared DTOs, queries, enums, and exceptions used across layers.

## Features

- JWT bearer authentication with access and refresh tokens
- Refresh-token rotation and logout support
- BCrypt password hashing
- Role-based authorization
- Custom authorization handler for owner/admin access rules
- Rate limiting for authentication, user, and order endpoints
- Swagger/OpenAPI documentation
- Sales and product reports
- Order lifecycle management

## Solution Structure

- `API_Layer/`
  - Controllers for `auth`, `categories`, `customers`, `orders`, `payment-methods`, `payments`, `products`, `refunds`, `reports`, `roles`, `tables`, and `users`
  - JWT, CORS, Swagger, and rate-limit configuration
- `BusinessLogicLayer/`
  - Service interfaces and implementations
  - MediatR event/handler support
- `DataAccessLayer/`
  - Repository interfaces and SQL Server repositories
  - Database settings
- `Contracts/`
  - DTOs for requests and responses
  - Query objects for reports
  - Shared enums and exception types
- `DatabaseScripts/`
  - SQL helper scripts, including `RefundedPayments.sql`

## Prerequisites

- .NET SDK 8.0
- SQL Server
- Visual Studio 2022 or `dotnet` CLI

## Database

The API expects a SQL Server database named `ResturantDB`.

Default connection string:

```json
"Server=.;Database=ResturantDB;User Id=sa;Password=sa123456;TrustServerCertificate=True;"
```

Update this in [`API_Layer/appsettings.json`](API_Layer/appsettings.json) before running in your own environment.

If you need the stored procedure used by the repository layer, see [`DatabaseScripts/RefundedPayments.sql`](DatabaseScripts/RefundedPayments.sql).

## Configuration Notes

- JWT issuer and audience are configured in [`API_Layer/Program.cs`](API_Layer/Program.cs).
- The current JWT signing key is hardcoded in the API startup file. Replace it with a secure secret or environment variable before deploying.
- Swagger is enabled in Development only.
- The default development endpoints are:
  - `http://localhost:5074`
  - `https://localhost:7186`

## Run Locally

1. Restore packages:

   ```bash
   dotnet restore
   ```

2. Make sure SQL Server is running and the `ResturantDB` database exists.

3. Update the connection string in [`API_Layer/appsettings.json`](API_Layer/appsettings.json) if needed.

4. Run the API project:

   ```bash
   dotnet run --project API_Layer/API_Layer.csproj
   ```

5. Open Swagger:

   - `https://localhost:7186/swagger`
   - or `http://localhost:5074/swagger`

## Main API Areas

- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/orders`
- `POST /api/orders`
- `PATCH /api/orders/{id}/preparing`
- `PATCH /api/orders/{id}/ready`
- `PATCH /api/orders/{id}/cancelled`
- `POST /api/orders/{id}/items`
- `GET /api/payments`
- `POST /api/payments/orders/{orderId}/payments`
- `GET /api/reports/top-products`
- `GET /api/reports/sales-comparison`
- `GET /api/reports/sales-details`
- `GET /api/reports/sales-trends`

Other controllers cover categories, customers, payment methods, products, refunds, roles, tables, and users.

## Authentication

Most endpoints require a Bearer token.

Example header:

```http
Authorization: Bearer <your-token>
```

## Notes for Contributors

- Follow the existing layered structure when adding new features.
- Put shared contracts in `Contracts`.
- Keep database access inside `DataAccessLayer`.
- Keep business rules inside `BusinessLogicLayer`.
- Expose only HTTP-specific concerns from `API_Layer`.

## License

Add your preferred license here before publishing the repository publicly.
