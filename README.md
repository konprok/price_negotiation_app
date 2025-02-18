# API Documentation

## Technologies Used

- **ASP.NET Core** - Web framework for building APIs.
- **Entity Framework Core** - ORM for database interactions.
- **FluentValidation** - Library for model validation.
- **NUnit & NSubstitute** - Testing framework and mocking library.
- **PostgreSQL** - Database used in the project.
- **Swagger (Swashbuckle)** - API documentation generator.

---

## Setup & Run

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Installation & Running

1. Clone the repository:
   ```sh
   git clone https://github.com/konprok/price_negotiation_app.git
   cd price_negotiation_app
   ```

2. Configure database connection in `appsettings.json`:
   ```json
   "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Database=your_db;Username=your_user;Password=your_password"
   }
   ```

3. Apply migrations:
   ```sh
   dotnet ef database update
   ```

4. Run the API:
   ```sh
   dotnet run
   ```

5. Open Swagger UI (for API testing):
    - [http://localhost:5231/swagger](http://localhost:5231/swagger) (default URL)

---


## UserController

### POST `/users/register`
**Description:** Register a new user.

#### Body (JSON):
```json
{
  "userName": "string",
  "email": "string",
  "password": "string"
}
```

#### Responses:
- ✅ `200 OK` - The user was successfully registered.
- ❌ `400 Bad Request` - Invalid input data.
- ❌ `500 Internal Server Error` - Internal server error.

---

### POST `/users/login`
**Description:** User login.

#### Body (JSON):
```json
{
  "email": "string",
  "password": "string"
}
```

#### Responses:
- ✅ `200 OK` - The user successfully logged in.
- ❌ `400 Bad Request` - Invalid login credentials.
- ❌ `404 Not Found` - User not found.
- ❌ `500 Internal Server Error` - Internal server error.

---

## ProductController

### POST `/products`
**Description:** Add a new product.

#### Body (JSON):
```json
{
  "userId": "Guid",
  "product": {
    "name": "string",
    "description": "string",
    "price": 0.0
  }
}
```

#### Responses:
- ✅ `200 OK` - The product was successfully added.
- ❌ `400 Bad Request` - Invalid input data.
- ❌ `404 Not Found` - User not found.
- ❌ `500 Internal Server Error` - Internal server error.

---

### GET `/products/{productId}`
**Description:** Retrieve product details.

#### Parameters:
- `productId` (long) - The product ID.

#### Responses:
- ✅ `200 OK` - Returns product details.
- ❌ `404 Not Found` - Product not found.
- ❌ `500 Internal Server Error` - Internal server error.

---

### GET `/products/user/{userId}`
**Description:** Retrieve products belonging to a specific user.

#### Parameters:
- `userId` (Guid) - The user ID.

#### Responses:
- ✅ `200 OK` - List of the user's products.
- ❌ `404 Not Found` - User not found.
- ❌ `500 Internal Server Error` - Internal server error.

---

### GET `/products/all`
**Description:** Retrieve all products.

#### Responses:
- ✅ `200 OK` - List of all products.
- ❌ `500 Internal Server Error` - Internal server error.

---

## NegotiationController

### POST `/negotiations/proposition`
**Description:** Submit a new negotiation proposal.

#### Body (JSON):
```json
{
  "clientId": "Guid",
  "productId": "long",
  "price": 0.0
}
```

#### Responses:
- ✅ `200 OK` - The proposal was successfully submitted.
- ❌ `400 Bad Request` - Invalid input data.
- ❌ `404 Not Found` - User or product not found.
- ❌ `409 Conflict` - The proposal conflicts with an existing one.
- ❌ `500 Internal Server Error` - Internal server error.

---

### PATCH `/negotiations/proposition`
**Description:** Update the status of a negotiation proposal.

#### Body (JSON):
```json
{
  "userId": "Guid",
  "negotiationId": "long",
  "response": true
}
```

#### Responses:
- ✅ `200 OK` - The proposal was updated.
- ❌ `404 Not Found` - Negotiation not found.
- ❌ `500 Internal Server Error` - Internal server error.

---

### GET `/negotiations/user/{userId}`
**Description:** Retrieve a user's negotiations.

#### Parameters:
- `userId` (Guid) - The user ID.

#### Responses:
- ✅ `200 OK` - List of the user's negotiations.
- ❌ `500 Internal Server Error` - Internal server error.
