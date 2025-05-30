# Dockerfile for HRTool ASP.NET Core WebAPI
# Multi-stage build for efficiency and smaller images

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY HRTool.API/HRTool.API.csproj HRTool.API/
COPY HRTool.Application/HRTool.Application.csproj HRTool.Application/
COPY HRTool.Infrastructure/HRTool.Infrastructure.csproj HRTool.Infrastructure/
COPY HRTool.Domain/HRTool.Domain.csproj HRTool.Domain/
RUN dotnet restore HRTool.API/HRTool.API.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/HRTool.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app and SQLite database
COPY --from=build /app/publish .
COPY HRTool.API/app.db ./app.db

# (Optional) Use a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose the port defined in launchSettings.json (5134)
EXPOSE 5134

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5134

# (Optional) Allow SQLite path override
ENV ConnectionStrings__DefaultConnection="Data Source=app.db"

# Start the application
ENTRYPOINT ["dotnet", "HRTool.API.dll"]

# ---
# Build: docker build -t hr-api .
# Run:   docker run -d -p 5134:5134 hr-api
#
# To inspect the SQLite DB, you can copy it from the container or mount a volume.
# For development, you may use docker-compose to mount ./HRTool.API/app.db as a volume.
