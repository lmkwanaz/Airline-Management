# SkyFlow Terminal Manager

SkyFlow is a console-based Airport and Airline Management System built in C#.
It demonstrates advanced OOP principles and SQL Server persistence.

## OOP Pillars Demonstrated

- Inheritance: `User` is a base class with `AdminUser` and `GateAgentUser` implementations.
- Encapsulation: `Flight.Status` is read-only and can only change through methods like `StartBoarding()`, `DepartFlight()`, and `CancelFlight()`.
- Polymorphism: `DisplayDashboard()` is overridden per role to render role-specific menus.
- Abstraction: `IDataRepository` separates business flow from SQL Server data access implementation.

## Tech Stack

- .NET 9 Console App
- ADO.NET via `Microsoft.Data.SqlClient`
- SQL Server

## Setup

1. Run the SQL script in `sql/init_skyflow.sql` to create and seed the database.
2. Set the environment variable `SKYFLOW_CONNECTION_STRING`.
   Example:
   `Server=localhost;Database=SkyFlowDb;Trusted_Connection=True;TrustServerCertificate=True;`
3. Build and run from the solution root:

```powershell
dotnet build SkyFlow.sln --configfile .\NuGet.Config
dotnet run --project .\SkyFlow.Console\SkyFlow.Console.csproj
```

If `SKYFLOW_CONNECTION_STRING` is not set, the app defaults to:
`Server=localhost;Database=SkyFlowDb;Trusted_Connection=True;TrustServerCertificate=True;`

## Key Features

- Login screen authenticates against SQL Server `User` table
- Admin workflow:
   - Manage flights (create schedule with capacity)
   - View system overview (occupancy and flight status)
   - Add new staff members
   - Update flight status (Boarding, Boarded, Departed, Cancelled)
- Gate Agent workflow:
   - View flight manifest in aligned ASCII table format
   - Search passenger by PassengerId or PassportNumber and check in
   - Finalize gate operation by setting flight to Boarded or Departed
- Data is persisted in SQL Server and remains available after app restart

## Seed Login Credentials

- Administrator: username `admin`, password `admin123`
- Gate Agent: username `gate1`, password `gate123`

## Submission Checklist

- Include source code (this project)
- Include SQL seed/setup script (`sql/init_skyflow.sql`)
- Include your PDF report
- Zip a single folder and keep file size under 50MB
- Run and verify the app before uploading
