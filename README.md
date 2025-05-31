# HRTool Backend

A modular HR management backend built with ASP.NET Core WebAPI, Entity Framework Core, and SQLite, following Clean Architecture principles. It supports user management, authentication (JWT), notifications, company links, out-of-office tracking, and dashboard aggregation.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [Database & EF Core](#database--ef-core)
- [Repository Pattern](#repository-pattern)
- [Dependency Injection](#dependency-injection)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Error Handling & Logging](#error-handling--logging)
- [Swagger/OpenAPI](#swaggeropenapi)
- [Docker Support](#docker-support)
- [Seeding & Admin User](#seeding--admin-user)
- [Development & Contribution](#development--contribution)

---

## Architecture Overview

- **Clean Architecture (Onion):**  
  - **Domain:** Core business logic, entities, value objects, repository interfaces.
  - **Application:** Business services, DTOs, application logic.
  - **Infrastructure:** EF Core DbContext, repository implementations, data access.
  - **API (Presentation):** Controllers, authentication, middleware, startup/configuration.

- **SOLID Principles:** Each class has a single responsibility, and dependencies are inverted via interfaces.

---

## Project Structure
HRTool.Domain/           # Entities, value objects, repository interfaces
HRTool.Application/      # DTOs, business/application services
HRTool.Infrastructure/   # EF Core DbContext, repository implementations
HRTool.API/              # WebAPI controllers, middleware, startup, DI, Swagger, Auth
tests/                   # Unit and integration tests for all layers
---

## Domain Model

- **User:**  
  - `Id`, `FirstName`, `LastName`, `Email`, `Role` (enum: Admin/User), `DateOfBirth`, `Skills`, `Address` (value object), `Department`, `IsOutOfOffice`, `OutOfOfficeUntil`, `ManagerId`, `CurrentProject`, `CreatedAt`, `PasswordHash`
- **Role:** Enum (`Admin`, `User`)
- **Notification:** `Id`, `Title`, `Message`, `ExpiryDate`, `IsActive`
- **CompanyLink:** `Id`, `Title`, `Url`
- **Value Objects:** `Address` (`Street`, `City`, `Country`)

Repository interfaces for each aggregate root are defined in the Domain layer.

---

## Database & EF Core

- **Provider:** SQLite (file-based, easy for dev/test)
- **DbContext:** `HrDbContext` (in Infrastructure)
  - `DbSet<User>`, `DbSet<Notification>`, `DbSet<CompanyLink>`
  - Model configuration via Fluent API (unique email, string lengths, relationships, owned types for Address)
- **Migrations:** Use `dotnet ef migrations add <Name>` and `dotnet ef database update` to manage schema.

---

## Repository Pattern

- **Interfaces:** In Domain (e.g., `IUserRepository`, `INotificationRepository`, `ICompanyLinkRepository`, `IUnitOfWork`)
- **Implementations:** In Infrastructure, using EF Core, injected via DI.
- **Unit of Work:** For transactional consistency.

---

## Dependency Injection

- All services, repositories, and DbContext are registered in `Program.cs` using the built-in DI container.
- Example:  builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddDbContext<HrDbContext>(...);
builder.Services.AddScoped<UserService>();
---

## Authentication & Authorization

- **JWT Bearer Authentication:**  
  - Secure login via `/api/auth/login` (returns JWT on success)
  - Passwords are hashed using ASP.NET Core’s `PasswordHasher`
  - JWT secret key and admin password are set via environment variables (`JWT_KEY`, `HRTOOL_ADMIN_PASSWORD`)
  - Role-based authorization: `[Authorize(Roles = "Admin")]` for admin endpoints

---

## API Endpoints

- **Auth:** `/api/auth/login`
- **User Profile:** `/api/user/me` (GET/PUT), `/api/user/me/outofoffice` (PUT)
- **Admin User Management:** `/api/user/admin/users` (GET/POST/PUT)
- **Directory:** `/api/directory` (GET)
- **Notifications:** `/api/notifications` (GET), `/api/notifications/admin` (admin CRUD)
- **Company Links:** `/api/companylinks` (GET), `/api/companylinks/admin` (admin CRUD)
- **Dashboard:** `/api/dashboard` (GET, aggregates all dashboard data)
- **Birthdays/New Hires:** `/api/user/birthdays/today`, `/api/user/birthdays/tomorrow`, `/api/user/new?days=30`

All endpoints are documented with XML comments and appear in Swagger UI.

---

## Testing

- **Unit Tests:**  
  - Projects for Domain, Application, and Infrastructure layers using xUnit and FluentAssertions.
- **Integration Tests:**  
  - API-level tests using `WebApplicationFactory` and in-memory SQLite.
- **Run all tests:**  dotnet test
---

## Error Handling & Logging

- **Global Exception Handler:**  
  - Custom middleware returns standardized JSON error responses and logs exceptions.
- **Logging:**  
  - Uses ASP.NET Core `ILogger` for info, warnings, and errors (no sensitive data logged).

---

## Swagger/OpenAPI

- **Enabled in Development:**  
  - `/swagger` endpoint for interactive API docs.
  - JWT Bearer authentication supported in Swagger UI.
  - XML comments included for models and endpoints.

---

## Docker Support

- **Dockerfile:**  
  - Multi-stage build for efficient images.
  - Exposes ports 8080/8081.
  - Uses environment variables for configuration.
- **Build & Run:**  docker build -t hrtool-api .
docker run -d -p 8080:8080 -e JWT_KEY=... -e HRTOOL_ADMIN_PASSWORD=... hrtool-api
---

## Seeding & Admin User

- **Admin Seeder:**  
  - On first run, seeds a default admin user if none exists.
  - Admin password is set via `HRTOOL_ADMIN_PASSWORD` environment variable.
- **Test Data Seeder:**  
  - Optionally seeds test users and data if enabled in config.

---

## Development & Contribution

- **Requirements:** .NET 8 SDK, SQLite, Docker (optional)
- **Setup:**  
  1. Clone the repo
  2. Set environment variables:  
     - `JWT_KEY` (32+ chars, for JWT signing)  
     - `HRTOOL_ADMIN_PASSWORD` (for initial admin)
  3. Run migrations:   ```
 dotnet ef database update --project HRTool.Infrastructure
 ```  4. Start the API:   ```
 dotnet run --project HRTool.API
 ```  5. Access Swagger UI at `https://localhost:port/swagger`

- **Contributing:**  
  - Follow Clean Architecture principles.
  - Add XML comments to all public models and endpoints.
  - Write or update tests for new features.
  - Keep controllers thin; put business logic in services.

---

## Further Reading

- [Clean Architecture (Code Maze)](https://code-maze.com/net-core-web-development-part4/)
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Swagger/OpenAPI](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle)

---

**For any questions or issues, please open an issue or contact the maintainers.**

---

## AI Assistance

This project was developed with **assistance from AI tools**, including [ChatGPT (o4-mini-high)](https://openai.com/chatgpt) and [GitHub Copilot](https://github.com/features/copilot).
AI was used for generating code, documentation, refactoring suggestions, troubleshooting, and accelerating overall development. All code and documentation were reviewed and adapted as needed by the project maintainer.

---
