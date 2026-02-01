# Stage 1: Build the frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app/client

# Copy package files
COPY yakudza-docs/client/package*.json ./

# Install dependencies
RUN npm ci

# Copy client source code
COPY yakudza-docs/client/ ./

# Build the frontend
RUN npm run build

# Stage 2: Build the backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /app

# Copy solution and project files
COPY yakudza-docs.sln ./
COPY yakudza-docs/yakudza-docs.csproj ./yakudza-docs/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the backend code
COPY yakudza-docs/ ./yakudza-docs/

# Copy the built frontend from the previous stage
COPY --from=frontend-build /app/client/dist ./yakudza-docs/client/dist

# Build and publish the application
RUN dotnet publish yakudza-docs/yakudza-docs.csproj -c Release -o /app/publish

# Stage 3: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=backend-build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "yakudza-docs.dll"]
