# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["net-advance-program.sln", "."]
COPY ["src/CartService/CartService.API/CartService.API.csproj", "src/CartService/CartService.API/"]
COPY ["src/CartService/CartService.Application/CartService.Application.csproj", "src/CartService/CartService.Application/"]
COPY ["src/CartService/CartService.Domain/CartService.Domain.csproj", "src/CartService/CartService.Domain/"]
COPY ["src/CartService/CartService.Infrastructure/CartService.Infrastructure.csproj", "src/CartService/CartService.Infrastructure/"]
COPY ["Directory.Build.props", "."]

# Restore dependencies
RUN dotnet restore "src/CartService/CartService.API/CartService.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/CartService/CartService.API"
RUN dotnet build "CartService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CartService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy published application
COPY --from=publish /app/publish .

# Run migrations and start the application
ENTRYPOINT ["dotnet", "CartService.API.dll"]

