# BookShareHub

A full-stack application built with **.NET 10** and **Angular 21**, following Clean Architecture principles and Test-Driven Development (TDD).  
It provides authentication (JWT) and CRUD operations for books.

---

Project Structure

```
BookShareHub.sln
 ├── BookShareHub.Domain
 ├── BookShareHub.Application
 ├── BookShareHub.AuthAPI
 ├── BookShareHub.BooksAPI
 ├── BookShareHub.Infrastructure
 ├── BookShareHub.UI (Angular 21)
 └── BookShareHub.Tests

```

---

## Technology Stack

- .NET 10 Web API
- C#
- Angular 21
- SQL Server
- Swagger / OpenAPI
- xUnit (unit testing)

---

## Requirements

- .NET 10 SDK
- SQL Server (local or remote)
- Node.js + Angular CLI
- Visual Studio / VS Code

---

## Installation & Setup

### Clone the repository

```bash
git clone <repo-url>
cd BookShareHub
```

---

### Backend

Configure database connection in appsettings.Development.json

```
"ConnectionStrings": {
  "BookShareHubDatabase": "Server=localhost;Database=BookShareHub;User Id=yourUser;Password=yourPassword;"
}
```

Run APIs locally:

```
dotnet run --project BookShareHub.AuthAPI
dotnet run --project BookShareHub.BooksAPI
```

Swagger will be available at:

    Auth API → http://localhost:7007/swagger/index.html

    Books API → http://localhost:7016/swagger/index.html

---

### API Endpoints

Users (AuthAPI)
| Method | Endpoint | Description |
| --- | --- | --- |
| POST | /api/Users/register | Register a new user |
| POST | /api/Users/login | Authenticate and return JWT |
| POST | /api/Users/logout | Logout (client discards JWT) |
| GET | /api/Users/me | Get current authenticated user info |
| PATCH | /api/Users/{id} | Update own profile (partial update) |
| DELETE | /api/Users/{id} | Delete user by ID |

Books (BooksAPI)
| Method | Endpoint | Description |
| --- | --- | --- |
| POST | /api/Books | Create a new book |
| GET | /api/Books/{id} | Get book by ID |
| GET | /api/Books | Get all books |
| PATCH | /api/Books/{id} | Update book (partial) |
| DELETE | /api/Books/{id} | Delete book by ID |

---

### Tests

Unit tests implemented with xUnit in BookShareHub.Tests.

Coverage includes:

- User registration and login
- JWT authentication
- CRUD operations for books
- Validation rules and error scenario

---

### Frontend

Frontend will be available at http://localhost:4200.

```
cd BookShareHub.UI
npm install
ng serve
```

---

### Business Rules

- User must be authenticated to access book endpoints.
- Book requires Title and Author.
- User can only update their own profile (PATCH /api/Users/{id}).
- JWT token must be provided in Authorization: Bearer <token> header.
- Books marked as unavailable cannot be deleted.
