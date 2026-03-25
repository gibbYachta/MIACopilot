# MIACopilot

# Apprentice Management System

A .NET 10 Web API for managing apprentices, their work journals, and grades.

## Features

- **Apprentice Management** – Create, read, update, and delete apprentice records.
- **Work Journal** – Track daily work activities, hours worked, and skills practiced for each apprentice.
- **Grade Tool** – Record subject grades, calculate percentages, and view summary statistics per apprentice.

## Projects

| Project | Description |
|---------|-------------|
| `ApprenticeManagement.Api` | ASP.NET Core 10 Web API with SQLite persistence |
| `ApprenticeManagement.Tests` | xUnit unit tests for all service layers |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run the API

```bash
cd ApprenticeManagement.Api
dotnet run
```

The API will start on `https://localhost:5001` / `http://localhost:5000`.  
OpenAPI docs are available at `/openapi/v1.json` in development mode.

### Run Tests

```bash
dotnet test ApprenticeManagement.Tests
```

## API Endpoints

### Apprentices

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/apprentices` | List all apprentices |
| `GET` | `/api/apprentices/{id}` | Get apprentice by ID |
| `POST` | `/api/apprentices` | Create a new apprentice |
| `PUT` | `/api/apprentices/{id}` | Update an apprentice |
| `DELETE` | `/api/apprentices/{id}` | Delete an apprentice |

### Work Journal

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/apprentices/{id}/journal` | List journal entries |
| `GET` | `/api/apprentices/{id}/journal/{entryId}` | Get a journal entry |
| `POST` | `/api/apprentices/{id}/journal` | Add a journal entry |
| `PUT` | `/api/apprentices/{id}/journal/{entryId}` | Update a journal entry |
| `DELETE` | `/api/apprentices/{id}/journal/{entryId}` | Delete a journal entry |

### Grades

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/apprentices/{id}/grades` | List all grades |
| `GET` | `/api/apprentices/{id}/grades/summary` | Grade summary (avg, high, low) |
| `GET` | `/api/apprentices/{id}/grades/{gradeId}` | Get a specific grade |
| `POST` | `/api/apprentices/{id}/grades` | Add a grade |
| `PUT` | `/api/apprentices/{id}/grades/{gradeId}` | Update a grade |
| `DELETE` | `/api/apprentices/{id}/grades/{gradeId}` | Delete a grade |

## Data Model

### Apprentice
- `Id`, `FirstName`, `LastName`, `Email` (unique), `Program`, `StartDate`, `EndDate?`

### WorkJournalEntry
- `Id`, `ApprenticeId`, `Date`, `Title`, `Description`, `HoursWorked`, `Skills?`

### Grade
- `Id`, `ApprenticeId`, `Subject`, `Score`, `MaxScore`, `Percentage` (computed), `Date`, `Notes?`
