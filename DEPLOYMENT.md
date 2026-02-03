# Deployment Guide

## Docker Deployment

### Environment Variables

#### Backend Configuration

Set these environment variables when running the container:

- `ASPNETCORE_URLS` - The URLs the app listens on (default: `http://+:8080`)
- `ASPNETCORE_ENVIRONMENT` - Environment name (default: `Production`)
- `Cors__AllowedOrigins` - Semicolon-separated list of allowed CORS origins
  - Example: `http://yourdomain.com;https://yourdomain.com`
  - Required if accessing the API from a different domain/port

#### Frontend Build Configuration

- `VITE_API_BASE_URL` - API base URL for production build (default: `/api`)
  - Use `/api` (relative URL) when frontend is served by the same backend (recommended)
  - Use absolute URL (e.g., `http://api.yourdomain.com/api`) if frontend is served separately

### Building the Docker Image

```bash
# Build with default settings (frontend uses relative URLs)
docker build -t yakudza-docs .

# Build with custom API URL (if frontend served separately)
docker build --build-arg VITE_API_BASE_URL=http://api.yourdomain.com/api -t yakudza-docs .
```

### Running the Container

#### Default Configuration (Recommended)

Frontend served by backend on same domain:

```bash
docker run -d \
  -p 8080:8080 \
  -v yakudza-data:/app/data \
  yakudza-docs
```

Access the app at: `http://localhost:8080`

#### With Custom Domain

If deploying to a custom domain:

```bash
docker run -d \
  -p 8080:8080 \
  -v yakudza-data:/app/data \
  -e Cors__AllowedOrigins="http://yourdomain.com;https://yourdomain.com" \
  yakudza-docs
```

#### Separate Frontend/Backend Deployment

If frontend is served separately (e.g., via Nginx, CDN):

1. Build with custom API URL:
```bash
docker build --build-arg VITE_API_BASE_URL=http://api.yourdomain.com/api -t yakudza-docs .
```

2. Run with CORS configuration:
```bash
docker run -d \
  -p 8080:8080 \
  -v yakudza-data:/app/data \
  -e Cors__AllowedOrigins="http://frontend.yourdomain.com;https://frontend.yourdomain.com" \
  yakudza-docs
```

### Configuration File Override

You can also override settings using appsettings.Production.json:

Create `appsettings.Production.json`:
```json
{
  "Cors": {
    "AllowedOrigins": "http://yourdomain.com;https://yourdomain.com"
  },
  "Jwt": {
    "Key": "your-secure-key-here-at-least-32-characters",
    "Issuer": "YakudzaDocs",
    "Audience": "YakudzaDocsClient"
  }
}
```

Mount it when running:
```bash
docker run -d \
  -p 8080:8080 \
  -v yakudza-data:/app/data \
  -v $(pwd)/appsettings.Production.json:/app/appsettings.Production.json \
  yakudza-docs
```

## Security Considerations

- Change the default JWT key in production
- Use HTTPS in production
- Restrict CORS origins to only trusted domains
- Store sensitive data in environment variables, not in appsettings.json
- Regularly backup the SQLite database volume
