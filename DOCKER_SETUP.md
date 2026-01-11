# Docker Setup Documentation

## Task 1: Containerized External Dependencies

### External Dependencies Identified:
1. **RabbitMQ** - Message broker for asynchronous communication
2. **PostgreSQL** - SQL database for CatalogService
3. **PostgreSQL** - SQL database for CartService

### Container Images Used:
- `rabbitmq:3-management` - Pre-built RabbitMQ image with management UI
- `postgres:15-alpine` - Pre-built PostgreSQL 15 Alpine image (lightweight)

### Configuration Changes:

#### CatalogService
- Changed from `UseInMemoryDatabase` to `UseNpgsql` (PostgreSQL)
- Connection string: `Host=catalog-db;Port=5432;Database=catalogdb;Username=catalog_user;Password=catalog_password`
- Added `Npgsql.EntityFrameworkCore.PostgreSQL` package

#### CartService
- Changed from `InMemoryCartRepository` to `PostgresCartRepository` using EF Core
- Created `CartDbContext` for database operations
- Connection string: `Host=cart-db;Port=5432;Database=cartdb;Username=cart_user;Password=cart_password`
- Added `Npgsql.EntityFrameworkCore.PostgreSQL` package

#### RabbitMQ Configuration
- Both services now use container hostname `rabbitmq` instead of `localhost`
- Configuration via environment variables: `RabbitMQ:Host`, `RabbitMQ:Username`, `RabbitMQ:Password`

## Task 2: Docker Compose Setup

### Services in docker-compose.yml:

1. **rabbitmq** - Message broker
   - Ports: 5672 (AMQP), 15672 (Management UI)
   - Health check: `rabbitmq-diagnostics -q ping`

2. **catalog-db** - PostgreSQL for CatalogService
   - Port: 5433 (host) -> 5432 (container)
   - Database: `catalogdb`
   - User: `catalog_user`

3. **cart-db** - PostgreSQL for CartService
   - Port: 5434 (host) -> 5432 (container)
   - Database: `cartdb`
   - User: `cart_user`

4. **catalog-service** - CatalogService API
   - Port: 5064 (host) -> 8080 (container)
   - Depends on: `catalog-db`, `rabbitmq`
   - Environment variables for connection strings

5. **cart-service** - CartService API
   - Port: 5001 (host) -> 8080 (container)
   - Depends on: `cart-db`, `rabbitmq`
   - Environment variables for connection strings

### Dockerfile Explanation:

Both Dockerfiles use multi-stage build:

1. **Build stage** (`FROM mcr.microsoft.com/dotnet/sdk:8.0`)
   - Copies solution and project files
   - Restores NuGet packages
   - Builds the application

2. **Publish stage** (from build)
   - Publishes the application with Release configuration

3. **Runtime stage** (`FROM mcr.microsoft.com/dotnet/aspnet:8.0`)
   - Copies published files
   - Exposes port 8080
   - Sets `ASPNETCORE_URLS=http://+:8080`
   - Runs the application

## Commands to Build, Run, and Test

### 1. Build and Start All Services
```bash
docker-compose up -d --build
```

**Explanation:**
- `up` - Creates and starts containers
- `-d` - Runs in detached mode (background)
- `--build` - Builds images before starting

### 2. Check Service Status
```bash
docker-compose ps
```

**Explanation:**
- Shows status of all services
- Displays ports and health status

### 3. View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f catalog-service
docker-compose logs -f cart-service
```

**Explanation:**
- `logs` - Shows container logs
- `-f` - Follows log output (like `tail -f`)

### 4. Check Database Connections
```bash
# Catalog database
docker exec -it catalog-db psql -U catalog_user -d catalogdb -c "SELECT version();"

# Cart database
docker exec -it cart-db psql -U cart_user -d cartdb -c "SELECT version();"
```

**Explanation:**
- `exec` - Executes command in running container
- `-it` - Interactive terminal
- `psql` - PostgreSQL command-line client

### 5. Test CatalogService
```bash
# Check if service is running
curl http://localhost:5064/swagger/index.html

# Get products
curl http://localhost:5064/api/v1/Products

# Create a product
curl -X POST http://localhost:5064/api/v1/Products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","description":"Test","price":99.99,"categoryId":1}'
```

### 6. Test CartService
```bash
# Check if service is running
curl http://localhost:5001/swagger/index.html

# Get cart
curl http://localhost:5001/api/v1/Cart/test-user

# Add item to cart
curl -X POST http://localhost:5001/api/v1/Cart/test-user/items \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":2}'
```

### 7. Test Message Broker Integration
```bash
# Update a product in CatalogService
curl -X PUT http://localhost:5064/api/v1/Products/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Product","description":"Updated","price":199.99,"categoryId":1}'

# Check CartService logs for message consumption
docker-compose logs cart-service | grep "Received product update"
```

**Explanation:**
- Updating a product triggers a message to RabbitMQ
- CartService consumes the message and updates cart items

### 8. Stop All Services
```bash
docker-compose down
```

**Explanation:**
- Stops and removes containers
- Keeps volumes (data persists)

### 9. Stop and Remove Volumes
```bash
docker-compose down -v
```

**Explanation:**
- `-v` - Removes volumes (deletes database data)

### 10. Rebuild Specific Service
```bash
docker-compose build catalog-service
docker-compose up -d catalog-service
```

**Explanation:**
- `build` - Rebuilds the image
- `up -d` - Starts the service

## Health Checks

All services have health checks configured:
- **RabbitMQ**: `rabbitmq-diagnostics -q ping`
- **PostgreSQL**: `pg_isready -U <user> -d <database>`
- **Services**: `curl -f http://localhost:8080/swagger/index.html`

## Network Configuration

All services are on the `app-network` bridge network:
- Services can communicate using container names as hostnames
- Example: `catalog-service` can reach `catalog-db` using hostname `catalog-db`

## Environment Variables

Services use environment variables for configuration:
- `ConnectionStrings__CatalogDb` - Catalog database connection
- `ConnectionStrings__CartDb` - Cart database connection
- `RabbitMQ__Host` - RabbitMQ hostname
- `RabbitMQ__Username` - RabbitMQ username
- `RabbitMQ__Password` - RabbitMQ password

## Database Migrations

Both services use `EnsureCreated()` on startup:
- Creates database schema if it doesn't exist
- CatalogService seeds initial data (categories and products)

