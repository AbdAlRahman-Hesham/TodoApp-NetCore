Here’s a clean and professional version of your API documentation formatted as a `README.md` file for your Todo API project:

---

# Todo API

## Table of Contents

* [Project Setup](#project-setup)

  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
* [Configuration](#configuration)

  * [appsettings.json](#appsettingsjson)
  * [Environment Variables](#environment-variables)
* [API Endpoints](#api-endpoints)

  * [Account Endpoints](#account-endpoints)
  * [Todo Endpoints](#todo-endpoints)
* [Authentication](#authentication)
* [Testing with Postman](#testing-with-postman)

---

## Project Setup

### Prerequisites

* [.NET 7 SDK or later](https://dotnet.microsoft.com/en-us/download)
* SQL Server (or compatible database)
* [Postman](https://www.postman.com/) (for API testing)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/AbdAlRahman-Hesham/TodoApp-NetCore.git
   ```

2. Configure the database connection in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your-server;Database=TodoDb;Integrated Security=True;TrustServerCertificate=True;"
   }
   ```

3. Apply database migrations:

   ```bash
   dotnet ef database update
   ```

4. Run the application:

   ```bash
   dotnet run
   ```

---

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-S1F3JC4;Database=Todo-Db;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your_super_secret_key_here_which_should_be_long",
    "Issuer": "TodoApp",
    "Audience": "TodoAppUser",
    "DurationInMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.yourprovider.com",
    "SmtpPort": 587,
    "FromAddress": "noreply@yourdomain.com",
    "FromName": "TodoApp Notification",
    "Username": "your-smtp-username",
    "Password": "your-smtp-password",
    "EnableSsl": true,
    "UseDefaultCredentials": false
  },
  "AllowedOrigins": [
    "http://localhost:4200"
  ],
  "AllowedHosts": "*"
}
```

### Environment Variables

In production, move sensitive values to environment variables:

* `JWT_KEY`
* `DB_CONNECTION_STRING`
* `SMTP_PASSWORD`

---

## API Endpoints

### Account Endpoints

#### Register

* **URL**: `/api/Account/register`

* **Method**: `POST`

* **Request Body**:

  ```json
  {
    "userName": "string",
    "email": "user@example.com",
    "password": "string"
  }
  ```

* **Success Response**:

  ```json
  {
    "token": "jwt_token_here"
  }
  ```

#### Login

* **URL**: `/api/Account/login`

* **Method**: `POST`

* **Request Body**:

  ```json
  {
    "email": "user@example.com",
    "password": "string"
  }
  ```

* **Success Response**:

  ```json
  {
    "token": "jwt_token_here"
  }
  ```

#### Get User Info

* **URL**: `/api/Account/me`
* **Method**: `GET`
* **Headers**: `Authorization: Bearer {token}`
* **Success Response**:

  ```json
  {
    "id": "string",
    "email": "string",
    "userName": "string"
  }
  ```

---

### Todo Endpoints

#### Get All Todos

* **URL**: `/api/Todo`
* **Method**: `GET`
* **Headers**: `Authorization: Bearer {token}`
* **Query Parameters** *(optional)*:

  * `PageNumber`
  * `PageSize`
  * `Status`
  * `Priority`
  * `StartDate`
  * `EndDate`
  * `SearchTerm`
  * `IsOverdue`
  * `IsDueToday`
  * `IsDueThisWeek`
  * `SortBy`
  * `SortDescending`

#### Create Todo

* **URL**: `/api/Todo`
* **Method**: `POST`
* **Headers**: `Authorization: Bearer {token}`
* **Request Body**:

  ```json
  {
    "title": "string",
    "description": "string",
    "status": 0,
    "priority": 0,
    "dueDate": "2023-01-01T00:00:00"
  }
  ```

#### Update Todo

* **URL**: `/api/Todo`
* **Method**: `PUT`
* **Headers**: `Authorization: Bearer {token}`
* **Request Body**:

  ```json
  {
    "id": "string",
    "title": "string",
    "description": "string",
    "status": 0,
    "priority": 0,
    "dueDate": "2023-01-01T00:00:00"
  }
  ```

#### Delete Todo

* **URL**: `/api/Todo/{id}`
* **Method**: `DELETE`
* **Headers**: `Authorization: Bearer {token}`

#### Complete Todo

* **URL**: `/api/Todo/{id}/complete`
* **Method**: `PATCH`
* **Headers**: `Authorization: Bearer {token}`

---

## Authentication

The API uses **JWT (JSON Web Token)** authentication.

Include the token in the Authorization header for protected endpoints:

```http
Authorization: Bearer your_jwt_token_here
```

---

## Testing with Postman

1. Import the provided Postman collection.
2. Set the `base_url` variable to your development URL (default: `https://localhost:7158`).
3. Start with the **Register** or **Login** request to retrieve a token.
4. The token will be saved to `auth_token` for use in future requests.

### Example Workflow

* Register a new user
* Login to get a token
* Create a new todo
* Get all todos
* Update a todo
* Mark a todo as complete
* Delete a todo

---

