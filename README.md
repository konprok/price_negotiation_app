# API Documentation

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
