# Database Layer Specification (SQLite + Docker Volume)

## 📌 Purpose

This document describes the functional requirements for adding a database layer to an ASP.NET Core Web API application using SQLite and Docker volumes.

The implementation must provide persistent storage, domain models, and REST API endpoints for managing restaurant dish technical cards.

---

## 1. General Requirements

- The backend must use **SQLite** as the primary database.
- The database file must be stored inside a **Docker volume** to ensure persistence.
- Data must be preserved between container restarts.
- Database access must be implemented using Entity Framework Core.

---

## 2. Data Model

### 2.1 Roles

The system must support user roles:

| Field | Type | Description |
|-------|------|-------------|
| Id | Integer | Primary key |
| Name | String | Role name (`User`, `Admin`) |

Supported roles:
- User
- Admin

---

### 2.2 Users

The system must store application users.

| Field | Type | Description |
|-------|------|-------------|
| Id | Integer | Primary key |
| Login | String | Unique username |
| PasswordHash | Binary | Hashed password |
| PasswordSalt | Binary | Salt for hashing |
| RoleId | Integer | Reference to Role |

Requirements:
- Passwords must not be stored in plain text.
- Logins must be unique.
- Each user must have exactly one role.

---

### 2.3 Dish Technical Cards

The system must store technical cards for restaurant dishes.

| Field | Type | Description |
|-------|------|-------------|
| Id | Integer | Primary key |
| Name | String | Dish name |
| Description | String | Dish description |
| Image | Binary | Dish image (BLOB) |

Requirements:
- Each dish must have a unique identifier.
- Images must be stored directly in the database as binary data.
- Images are optional.

---

### 2.4 Dish Ingredients

Each dish must contain a list of ingredients with defined weights.

| Field | Type | Description |
|-------|------|-------------|
| Id | Integer | Primary key |
| DishTechCardId | Integer | Reference to dish |
| Name | String | Ingredient name |
| WeightGrams | Decimal | Weight in grams |

Requirements:
- Each ingredient belongs to exactly one dish.
- Ingredients must be removed when the related dish is deleted.

---

## 3. Initial Test Data

The system must be initialized with sample data:

### Roles
- User
- Admin

### Users
- At least one administrator account - (login admin, password - password) / later we will add functionality to init admin user in production
- At least one regular user account

### Dishes
- At least two example dish technical cards
- Each dish must include multiple ingredients
- Test images may be empty or minimal

The test data must be created automatically when the application starts for the first time.

---

## 4. API Requirements

The backend must expose REST endpoints for managing dish technical cards.

### 4.1 Dish Management

The API must support the following operations:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/dishes | Get all dishes |
| GET | /api/dishes/{id} | Get dish details |
| GET | /api/dishes/{id}/image | Get dish image |
| POST | /api/dishes | Create new dish |
| PUT | /api/dishes/{id} | Update existing dish |
| DELETE | /api/dishes/{id} | Delete dish |

---

### 4.2 Create / Update Dish

When creating or updating a dish, the following data must be supported:

- Dish name
- Description
- Ingredient list
- Ingredient weights
- Optional image (Base64 or binary upload)

Updating a dish must replace the existing ingredient list.

---

## 5. Data Validation

The system must validate input data:

- Dish name must not be empty.
- Ingredient names must not be empty.
- Ingredient weight must be greater than zero.
- Image size must be limited.
- Duplicate dish names should be prevented if possible.

---

## 6. Persistence Requirements

- Database file must be located in a directory mounted as a Docker volume.
- No data loss is allowed during container restarts.
- Database schema must be managed via migrations.

---

## 7. Security Requirements

- Passwords must be securely hashed.
- Sensitive data must not be exposed through API responses.
- User authentication and authorization should be supported in future versions.
- Admin-only operations must be possible to enforce later.

---

## 8. Performance and Scalability

- Database queries must be optimized for common operations.
- Large images must not significantly degrade API performance.
- List endpoints must not return binary image data.

---

## 9. Frontend Integration

The API must be designed to be easily consumed by a React + TypeScript frontend.

Requirements:

- JSON-based communication
- Stable DTO contracts
- Predictable error responses
- Support for pagination and filtering in future versions

---

## 10. Expected Outcome

After implementation, the system must provide:

- Persistent SQLite storage
- User and role management
- Dish technical card management
- Ingredient tracking
- RESTful API
- Sample test data
- Docker-ready persistence

The backend must be production-ready and suitable for extension with authentication, authorization, and advanced reporting features.
