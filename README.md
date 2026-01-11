# Message Broker Integration

## Task 1: RabbitMQ Setup

### Start RabbitMQ
```bash
docker-compose up -d
```

RabbitMQ Management UI: http://localhost:15672
- Username: `guest`
- Password: `guest`

## Task 2: Integration between Catalog and Cart Services

### CatalogService (Publisher)
- Port: 5064
- Swagger: http://localhost:5064/swagger
- Publishes messages when product is updated via `PUT /api/v1/Products/{id}`

### CartService (Consumer)
- Port: 5001
- Swagger: http://localhost:5001/swagger
- Listens to `product-updated-queue`
- Updates cart items when product changes

## Running Services

1. Start RabbitMQ:
```bash
docker-compose up -d
```

2. Start CartService:
```bash
cd src/CartService/CartService.API
dotnet run
```

3. Start CatalogService:
```bash
cd src/CatalogService/CatalogService.API
dotnet run
```

## Testing

### Update a product in CatalogService:
```bash
curl -X PUT http://localhost:5064/api/v1/Products/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Product Name",
    "description": "Updated description",
    "price": 199.99,
    "categoryId": 1
  }'
```

### Check CartService logs
The CartService will log when it receives and processes the product update message.

## Reliability Features

- **Retry Policy**: 3 retries with exponential backoff (1s, 2s, 4s)
- **Dead Letter Queue**: Failed messages after retries go to DLQ
- **Message Acknowledgments**: Messages are acknowledged after successful processing

## Code Style and Quality

### Task 1: Code Style Setup

**What was done:**
- Added `.editorconfig` with code style rules (Microsoft conventions)
- Created `Directory.Build.props` - shared configuration for all projects
- Enabled using statements sorting (`dotnet_sort_system_directives_first`)
- Added analyzers: StyleCop, SonarAnalyzer, Microsoft.CodeAnalysis.NetAnalyzers
- Configured git pre-push hook - checks formatting before push

**How to use:**
```bash
# Check formatting
dotnet format --verify-no-changes

# Fix formatting
dotnet format
```

### Task 2: SonarQube

**What was done:**
- Added SonarQube to `docker-compose.yml`
- Fixed Major+ issues in code (null checks, unused fields)

**How to run analysis:**
```bash
# Start SonarQube
docker-compose up -d sonarqube-db sonarqube

# Wait for SonarQube to start (check http://localhost:9000)

# Project analysis
dotnet tool install --global dotnet-sonarscanner
export PATH="$PATH:$HOME/.dotnet/tools"

# Create token in SonarQube UI (User > My Account > Security)
# Then run analysis:
dotnet sonarscanner begin /k:"net-advance-program" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR_TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR_TOKEN"
```

**Results:** http://localhost:9000/dashboard?id=net-advance-program
