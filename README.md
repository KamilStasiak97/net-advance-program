# Message Broker Integration

## Task 1: RabbitMQ Setup

### Start RabbitMQ
```bash
docker-compose up -d
```

RabbitMQ Management UI: http://localhost:15672
- Username: `guest`
- Password: `guest`

## Task 2: Services

### SenderService (Publisher)
- Port: 5203
- Swagger: http://localhost:5203/swagger
- Endpoint: `POST /api/message/product-updated`
- Publishes messages to RabbitMQ

### ReceiverService (Consumer)
- Port: 5062
- Swagger: http://localhost:5062/swagger
- Listens to `product-updated-queue`
- Processes messages with retry policy and DLQ

## Running Services

1. Start RabbitMQ:
```bash
docker-compose up -d
```

2. Start ReceiverService:
```bash
cd src/ReceiverService/ReceiverService.API
dotnet run
```

3. Start SenderService:
```bash
cd src/SenderService/SenderService.API
dotnet run
```

## Testing

### Send a message:
```bash
curl -X POST http://localhost:5203/api/message/product-updated \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "name": "Test Product",
    "price": 99.99
  }'
```

### Check ReceiverService logs
The ReceiverService will log when it receives and processes the message.

## Reliability Features

- **Retry Policy**: 3 retries with exponential backoff (1s, 2s, 4s)
- **Dead Letter Queue**: Failed messages after retries go to DLQ
- **Message Acknowledgments**: Messages are acknowledged after successful processing
