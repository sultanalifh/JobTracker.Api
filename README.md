## بِسْمِ ٱللَّٰهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ

# JobTracker

JobTracker is a backend application built with ASP.NET Core for tracking job applications throughout the recruitment process.

The project was created as a Software Engineering learning project focused on modern backend development practices rather than simple CRUD implementation.

Current goals:

* Clean backend architecture
* RESTful API design
* PostgreSQL integration
* Redis caching
* Docker containerization
* Service and Repository pattern
* Exception handling middleware
* Production-oriented development workflow

---

## Tech Stack

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Redis
* Docker & Docker Compose
* Swagger / OpenAPI

---

## Current Features

✅ Create job applications

✅ Retrieve all job applications

✅ Retrieve application by ID

✅ Update application status

✅ Track application lifecycle

Supported statuses:

* Applied
* Viewed
* Interview
* Offer
* Rejected

✅ Statistics endpoint

Examples:

* Total applications
* Applications by status

✅ Global exception handling middleware

✅ Repository pattern

✅ Service layer abstraction

✅ PostgreSQL persistence with Entity Framework Core

✅ Dockerized development environment

---

## Project Structure

```text
JobTracker.Api
├── Data
├── Dtos
├── Exceptions
├── Middlewares
├── Migrations
├── Models
├── Repositories
├── Services
├── Program.cs
├── Dockerfile
└── docker-compose.yml
```

---

## Example Job Application

```json
{
  "id": 1,
  "company": "OpenAI",
  "position": "Software Engineer",
  "siteLocation": "LinkedIn",
  "status": "Applied",
  "createdAt": "2026-06-15T10:00:00Z",
  "updatedAt": "2026-06-15T10:00:00Z"
}
```

---

## Running Locally

### Using Docker

```bash
docker compose up -d
```

### Using .NET CLI

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Swagger will be available after startup.

---

## Project Status

Current Version: v0.1

Current Focus:

* Backend Engineering Practice
* API Design
* Database Integration
* Docker Workflow

---

## Planned Features

* Redis caching improvements
* Integration testing
* Unit testing
* Request validation
* Authentication & Authorization
* Structured logging
* CI/CD pipeline
* Frontend dashboard
* Cloud deployment

---

## Learning Objectives

This project is primarily a Software Engineering learning project.

The goal is to explore concepts commonly used in real-world backend systems:

* Layered Architecture
* Dependency Injection
* Data Persistence
* Caching
* Error Handling
* Containerization
* Testing
* Deployment

---

## Notes

This project is actively evolving and serves as a practical exploration of backend engineering concepts.

Made with curiosity, persistence, and countless debugging sessions

Dr. Gestalt
