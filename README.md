# Food Safety Inspection Tracker

An ASP.NET Core MVC application for tracking food premises inspections, outcomes, and follow-ups.

## Features

- **Dashboard**: Aggregated statistics with filtering by Town and Risk Rating
- **Premises Management**: Create, edit, delete food premises
- **Inspections Management**: Create inspections (Pass/Fail based on score)
- **Follow-Ups Management**: Create follow-ups for failed inspections
- **Role-Based Access**: Admin, Inspector, and Viewer roles
- **Serilog Logging**: Console and file logging with daily rolling
- **Seed Data**: 12 premises, 25 inspections, 10 follow-ups

## Technologies

- .NET 10.0
- ASP.NET Core MVC
- Entity Framework Core (SQLite)
- ASP.NET Core Identity
- Serilog
- Bogus
- Bootstrap 5
- xUnit

## Default Users

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@foodsafety.gov | Admin123! |
| Inspector | inspector@foodsafety.gov | Inspector123! |
| Viewer | viewer@foodsafety.gov | Viewer123! |

## Setup Instructions

```bash
# Clone the repository
git clone https://github.com/Naranbat423/oop-s2-2-mvc-78577.git
cd oop-s2-2-mvc-78577

# Restore dependencies
dotnet restore

# Create and update database
cd FoodSafety.MVC
dotnet ef database update

# Run the application
dotnet run
