# .NET Web API Core Project

This repository contains the backend implementation for a full-stack portfolio/content management system. The project is built using **ASP.NET Core Web API** and leverages best practices for authentication, data modeling, and security.

##  Key Features

* **Secure Authentication:** Implements user registration and login using **ASP.NET Core Identity** and **JSON Web Tokens (JWT)** for secure, stateless API access.
* **Profile Management:** Provides endpoints for authenticated users to retrieve and update their personal details, including `FullName`, `Bio`, and their Base64-encoded `AvatarUrl`.
* **Content Management (CRUD):** Full **CRUD** (Create, Read, Update, Delete) functionality for `Post` entities (Portfolios/Blog Content).
* **Data Modeling:** Defines clear and efficient models (`AppUser`, `Post`) and uses **Data Transfer Objects (DTOs)** for clean data exchange.
* **Related Data Inclusion:** Post listing and detail endpoints automatically include related user data (`AuthorFullName`, `AuthorAvatarUrl`) using Entity Framework Core's `.Include()` for single-call efficiency.
* **Authorization & Ownership:** Ensures that content can only be created, updated, or deleted by the authorized owner (`[Authorize]`, `ClaimTypes.NameIdentifier`).

<br>

##  Tech Stack

* **Framework:** ASP.NET Core Web API
* **Language:** C#
* **Database:** Entity Framework Core (Code First approach)
* **Identity:** ASP.NET Core Identity
* **Security:** JWT Authentication
* **Utilities:** Custom Slug Helper for unique URL generation.

<br>

##  Endpoints Overview (Key API Routes)

| Method | Route | Description | Authentication |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Creates a new user account. | Anonymous |
| `POST` | `/api/auth/login` | Authenticates a user and returns a JWT token. | Anonymous |
| `GET` | `/api/users/me` | Retrieves the current user's full profile details. | Authorized |
| `PUT` | `/api/users/me` | Updates the current user's profile (`FullName`, `Bio`, `AvatarUrl`). | Authorized |
| `GET` | `/api/posts` | Retrieves a paginated list of published posts (includes author details). | Anonymous |
| `GET` | `/api/posts/{slug}` | Retrieves a single post by its unique slug (includes author details). | Anonymous* |
| `POST` | `/api/posts` | Creates a new post. | Authorized |
| `PUT` | `/api/posts/{id:int}` | Updates an existing post by ID (requires ownership). | Authorized |
| `DELETE` | `/api/posts/{id:int}` | Deletes a post by ID (requires ownership). | Authorized |

> *Note: Unpublished posts (`IsPublished = false`) are only accessible by the post's author.*

<br>

##  Setup and Installation

### Prerequisites

* .NET SDK (v6.0 or newer)
* A configured database (e.g., SQL Server, SQLite, or PostgreSQL).

### Steps

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/zisansarac/.NET-WebAPI.git](https://github.com/zisansarac/.NET-WebAPI.git)
    cd .NET-WebAPI
    ```
2.  **Configure Connection:** Update your database connection string in `appsettings.json`.
3.  **Run Migrations:** Apply all necessary Entity Framework Core migrations to your database:
    ```bash
    dotnet ef database update
    ```
4.  **Run the API:**
    ```bash
    dotnet run
    ```
    The API should start running, typically accessible at `https://localhost:7xxx` or `https://localhost:5001`.
