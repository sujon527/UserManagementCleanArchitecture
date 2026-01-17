# User Management System (Clean Architecture)

A robust User Management system built with **ASP.NET Core 8.0**, **C#**, and **MongoDB**, following the principles of **Clean Architecture**.

## 🏗️ Architecture Overview

Theoretical layers used in this project:

- **Domain**: Core entities, value objects, and repository interfaces. No external dependencies.
- **Application**: Business logic services (`IUserService`, `IAdminService`), DTOs, and request validators (FluentValidation).
- **Infrastructure**: MongoDB persistence implementation, database context, and security services (BCrypt).
- **API**: ASP.NET Core Controllers, global exception handling, and dependency injection configuration.

## 🚀 Features

- **User Registration**: Identity uniqueness (Email/Username/Phone), normalization, and complex password validation.
- **Profile Management**: View and update self-profile with optimistic concurrency.
- **RBAC**: Role-based access control for administrative tasks.
- **Admin Operations**: Search users with paging/filtering and manage user status (Activate/Deactivate).
- **Audit Logging**: Comprehensive audit trail for security-sensitive operations.
- **Security**: BCrypt hashing and masked PII data in responses.

## 🛠️ Technology Stack

- **Backend**: .NET 8.0
- **Database**: MongoDB (via MongoDB.Driver)
- **Validation**: FluentValidation
- **Security**: BCrypt.Net-Next
- **Documentation**: Swagger/OpenAPI

## 🚦 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (running locally or a cloud URI)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/sujon527/UserManagementCleanArchitecture.git
   ```

2. Navigate to the project directory:
   ```bash
   cd UserManagementCleanArchitecture
   ```

3. Update the MongoDB connection string in `UserManagement.API/appsettings.json`:
   ```json
   "MongoDB": {
     "ConnectionString": "mongodb://localhost:27017",
     "DatabaseName": "UserManagementDb"
   }
   ```

4. Build and run:
   ```bash
   dotnet build
   dotnet run --project UserManagement.API
   ```

5. Open Swagger UI: `http://localhost:<port>/swagger`

## ✅ Design Principles Applied

- **Dependency Inversion**: High-level modules do not depend on low-level modules; both depend on abstractions.
- **Separation of Concerns**: Each layer has a specific responsibility, making the system easy to maintain and test.
- **Testability**: Independent domain and application layers allow for extensive unit testing without external dependencies.
