# Yakudza Docs Client Setup

## Development

### Start the development server:

```bash
npm run dev
```

The app will be available at `http://localhost:5173`

### Backend Proxy

The Vite dev server is configured to proxy API requests from `/api` to `http://localhost:8080/api`.

Make sure your backend server is running on port 8080 before starting the frontend.

## Available Pages

- **`/feed`** (default) - Browse and search dishes
- **`/dish/new`** - Create a new dish
- **`/dish/:id`** - View and edit a dish
- **`/login`** - Login page (UI only, not functional yet)
- **`/init-admin`** - Initialize admin account (UI only, not functional yet)

## Features

✅ Dark theme by default
✅ Search dishes
✅ Create new dishes with images and ingredients
✅ Edit existing dishes
✅ Delete dishes
✅ Responsive design
✅ Pagination

## Tech Stack

- **React 19** with TypeScript
- **React Router** for routing
- **Tailwind CSS** for styling
- **Vite** for build tooling

## Building for Production

```bash
npm run build
```

The built files will be in the `dist/` folder.

## Notes

- Authentication is not yet implemented on the backend, so all pages are accessible without login
- The login and init-admin pages are UI-only placeholders for future implementation
- Images are uploaded as base64 strings to the API
