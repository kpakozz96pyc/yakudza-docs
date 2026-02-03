# Authentication Setup Guide

## Overview

The application now has full JWT-based authentication for both backend and frontend.

## Changes Made

### Backend

1. **JWT Authentication**
   - Added JWT token generation and validation
   - Configured ASP.NET Core authentication middleware
   - JWT settings in `appsettings.json`

2. **Auth Endpoints**
   - `POST /api/auth/login` - User login
   - `POST /api/auth/init-admin` - Create first admin account
   - `GET /api/auth/check-admin` - Check if admin exists

3. **Protected Endpoints**
   - Create, Update, Delete dish endpoints now require authentication
   - GET endpoints remain public

4. **Database Migration**
   - Removed default admin user from seed data
   - Admin must be created via `/api/auth/init-admin`
   - Default "user" account still exists (login: `user`, password: `password`)

### Frontend

1. **Authentication Context**
   - Auth state management with React Context
   - Token storage in localStorage
   - Auto-login on page refresh

2. **Protected Routes**
   - Create/Edit dish pages require authentication
   - Automatic redirect to login page

3. **UI Updates**
   - Functional login page
   - Functional init admin page
   - Logout button in feed page
   - User info display
   - Create button only shown when authenticated

## First-Time Setup

1. **Start the backend**:
   ```bash
   cd yakudza-docs
   dotnet ef database update  # Apply migrations
   dotnet run
   ```

2. **Start the frontend**:
   ```bash
   cd client
   npm run dev
   ```

3. **Initialize Admin Account**:
   - Navigate to `http://localhost:5173`
   - Click "First time? Initialize admin account"
   - Create your admin credentials
   - You'll be automatically logged in

## Testing Authentication

### Test with existing user:
- Login: `user`
- Password: `password`
- Role: User

### Test admin creation:
1. Go to `/init-admin`
2. Create admin account
3. Try creating/editing/deleting dishes

### Test protected routes:
1. Logout
2. Try to access `/dish/new` - should redirect to login
3. Login and try again - should work

## API Usage

### Login Request
```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"login":"user","password":"password"}'
```

### Create Dish with Auth
```bash
# First get token from login
TOKEN="your_jwt_token_here"

curl -X POST http://localhost:8080/api/dishes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "name":"Test Dish",
    "description":"Test description",
    "ingredients":[{"name":"Test","weightGrams":100}]
  }'
```

## Security Notes

- JWT secret key is in `appsettings.json` (change in production!)
- Tokens expire after 7 days
- Passwords are hashed using HMACSHA512
- CORS is configured for `http://localhost:5173` and `http://localhost:3000`

## Troubleshooting

### "Admin already exists" error
- An admin user is already in the database
- Use the login page instead
- Or delete the database file and restart

### "Unauthorized" on protected endpoints
- Make sure you're logged in
- Check that token is being sent in Authorization header
- Token may have expired (7 days)

### Frontend shows "Login failed"
- Check backend is running on port 8080
- Check browser console for error details
- Verify credentials are correct
