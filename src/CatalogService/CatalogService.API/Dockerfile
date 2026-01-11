# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["net-advance-program.sln", "."]
COPY ["src/CatalogService/CatalogService.API/CatalogService.API.csproj", "src/CatalogService/CatalogService.API/"]
COPY ["src/CatalogService/CatalogService.Application/CatalogService.Application.csproj", "src/CatalogService/CatalogService.Application/"]
COPY ["src/CatalogService/CatalogService.Domain/CatalogService.Domain.csproj", "src/CatalogService/CatalogService.Domain/"]
COPY ["src/CatalogService/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "src/CatalogService/CatalogService.Infrastructure/"]
COPY ["Directory.Build.props", "."]

# Restore dependencies
RUN dotnet restore "src/CatalogService/CatalogService.API/CatalogService.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/CatalogService/CatalogService.API"
RUN dotnet build "CatalogService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CatalogService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy published application
COPY --from=publish /app/publish .

# Run migrations and start the application
ENTRYPOINT ["dotnet", "CatalogService.API.dll"]

