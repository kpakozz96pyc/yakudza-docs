# Yakudza Docs API Documentation

## Overview

This API provides endpoints for managing restaurant dish technical cards with ingredients.

Base URL: `http://localhost:8080/api`

## Authentication

The API uses JWT (JSON Web Token) bearer authentication for protected endpoints.

**Protected Endpoints:**
- POST `/api/dishes` - Create dish
- PUT `/api/dishes/{id}` - Update dish
- DELETE `/api/dishes/{id}` - Delete dish

**Public Endpoints:**
- GET `/api/dishes` - Get all dishes
- GET `/api/dishes/{id}` - Get dish details
- GET `/api/dishes/{id}/image` - Get dish image
- POST `/api/auth/login` - Login
- POST `/api/auth/init-admin` - Initialize first admin
- GET `/api/auth/check-admin` - Check if admin exists

## Database Initialization

The application automatically initializes the database with seed data on first startup:

### Default Users
- **User**: login: `user`, password: `password`
- **Admin**: Use `/api/auth/init-admin` to create the first admin account

### Sample Dishes
- Борщ (Ukrainian Borscht)
- Оливье (Olivier Salad)

## Authentication Endpoints

### 1. Login

**POST** `/api/auth/login`

Authenticate a user and receive a JWT token.

**Request Body:**
```json
{
  "login": "user",
  "password": "password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "login": "user",
  "role": "User"
}
```

**Error Responses:**
- `401 Unauthorized` - Invalid login or password

---

### 2. Initialize Admin

**POST** `/api/auth/init-admin`

Create the first admin account. Only works if no admin exists.

**Request Body:**
```json
{
  "login": "admin",
  "password": "SecurePassword123"
}
```

**Validation Rules:**
- `login` - Required, 1-100 characters
- `password` - Required, minimum 6 characters

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "login": "admin",
  "role": "Admin"
}
```

**Error Responses:**
- `400 Bad Request` - Admin already exists or validation errors

---

### 3. Check Admin Exists

**GET** `/api/auth/check-admin`

Check if an admin user exists in the system.

**Response:**
```json
true
```

---

## Using Authentication

To access protected endpoints, include the JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Example using curl:
```bash
curl -H "Authorization: Bearer YOUR_TOKEN_HERE" \
     -X POST \
     -H "Content-Type: application/json" \
     -d '{"name":"New Dish","description":"...","ingredients":[...]}' \
     http://localhost:8080/api/dishes
```

---

## Dish Endpoints

### 1. Get All Dishes (with Pagination and Search)

**GET** `/api/dishes`

Returns a paginated list of dishes.

**Query Parameters:**
- `page` (optional, default: 1) - Page number (min: 1)
- `pageSize` (optional, default: 10) - Items per page (min: 1, max: 100)
- `search` (optional) - Search by dish name or description

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Борщ",
      "description": "Классический украинский борщ с говядиной и сметаной",
      "hasImage": false
    }
  ],
  "totalCount": 2,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

**Examples:**
- Get first page: `GET /api/dishes?page=1&pageSize=10`
- Search: `GET /api/dishes?search=борщ`

---

### 2. Get Dish Details

**GET** `/api/dishes/{id}`

Returns detailed information about a specific dish including ingredients.

**Response:**
```json
{
  "id": 1,
  "name": "Борщ",
  "description": "Классический украинский борщ с говядиной и сметаной",
  "hasImage": false,
  "ingredients": [
    {
      "id": 1,
      "name": "Говядина",
      "weightGrams": 300
    },
    {
      "id": 2,
      "name": "Свекла",
      "weightGrams": 200
    }
  ]
}
```

**Error Responses:**
- `404 Not Found` - Dish with specified ID does not exist

---

### 3. Get Dish Image

**GET** `/api/dishes/{id}/image`

Returns the dish image as binary data (JPEG format).

**Response:**
- Content-Type: `image/jpeg`
- Binary image data

**Error Responses:**
- `404 Not Found` - Dish with specified ID does not exist or has no image

---

### 4. Create Dish

**POST** `/api/dishes`

**🔒 Requires Authentication**

Creates a new dish with ingredients.

**Request Body:**
```json
{
  "name": "Пельмени",
  "description": "Традиционные русские пельмени с мясом",
  "imageBase64": "base64_encoded_image_data_here",
  "ingredients": [
    {
      "name": "Фарш говяжий",
      "weightGrams": 500
    },
    {
      "name": "Тесто",
      "weightGrams": 300
    }
  ]
}
```

**Validation Rules:**
- `name` - Required, 1-200 characters
- `description` - Required, max 2000 characters
- `imageBase64` - Optional, Base64 encoded image, max 10MB
- `ingredients` - Required, at least one ingredient
- `ingredients[].name` - Required, 1-200 characters
- `ingredients[].weightGrams` - Required, must be greater than 0

**Response:**
- Status: `201 Created`
- Location header with URL to the created dish
- Body: Same as Get Dish Details response

**Error Responses:**
- `400 Bad Request` - Validation errors or invalid image format/size

---

### 5. Update Dish

**PUT** `/api/dishes/{id}`

**🔒 Requires Authentication**

Updates an existing dish. Replaces all ingredients with the new list.

**Request Body:**
```json
{
  "name": "Пельмени домашние",
  "description": "Традиционные русские пельмени с мясом (обновлено)",
  "imageBase64": "base64_encoded_image_data_here",
  "ingredients": [
    {
      "name": "Фарш говяжий",
      "weightGrams": 450
    },
    {
      "name": "Фарш свиной",
      "weightGrams": 150
    },
    {
      "name": "Тесто",
      "weightGrams": 300
    }
  ]
}
```

**Validation Rules:** Same as Create Dish

**Response:**
- Status: `200 OK`
- Body: Updated dish details

**Error Responses:**
- `404 Not Found` - Dish with specified ID does not exist
- `400 Bad Request` - Validation errors

**Note:** If `imageBase64` is not provided, the existing image is preserved.

---

### 6. Delete Dish

**DELETE** `/api/dishes/{id}`

**🔒 Requires Authentication**

Deletes a dish and all its ingredients (cascade delete).

**Response:**
- Status: `204 No Content`

**Error Responses:**
- `404 Not Found` - Dish with specified ID does not exist

---

## Running the Application

### Using Docker (Recommended)

```bash
# Build the image
docker build -t yakudza-docs .

# Create a named volume for database persistence
docker volume create yakudza-data

# Run the container
docker run -d \
  --name yakudza-docs \
  -p 8080:8080 \
  -v yakudza-data:/app/data \
  --restart unless-stopped \
  yakudza-docs

# View logs
docker logs -f yakudza-docs

# Stop the container
docker stop yakudza-docs

# Start the container again (data is persisted)
docker start yakudza-docs

# Remove the container (volume is preserved)
docker rm yakudza-docs

# Remove the volume (deletes all data)
docker volume rm yakudza-data
```

The application will be available at `http://localhost:8080`

### Local Development

```bash
cd yakudza-docs
dotnet run
```

## Database Location

- **Production (Docker)**: `/app/data/yakudza.db`
- **Development**: `./yakudza.db`

## Recent Changes

### Version 2.0 - Authentication & Authorization

- ✅ JWT-based authentication
- ✅ Login endpoint
- ✅ Init admin functionality (create first admin account)
- ✅ Protected endpoints for create/update/delete operations
- ✅ Role-based authorization (Admin/User roles)
- ✅ Frontend authentication with React context
- ✅ Protected routes in frontend

## Future Enhancements

- Advanced filtering and sorting
- Batch operations
- Image optimization and thumbnails
- User management endpoints (for admins)
- Password reset functionality
