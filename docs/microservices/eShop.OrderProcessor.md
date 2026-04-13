# `eShop.OrderProcessor` Microservice

## Overview

### **Responsibility:**

- Run a background worker that continuously checks ordering data for orders whose grace period has elapsed.
- Publish `GracePeriodConfirmedIntegrationEvent` messages to the event bus for each eligible order.
- Bridge ordering persistence and event-driven processing by querying PostgreSQL and emitting RabbitMQ integration events.

### Dependence to other services

- **PostgreSQL** (`ordering.orders`) to read submitted orders and grace-period timing.
- **RabbitMQ** to publish integration events through `eShop.EventBusRabbitMQ`.
- **Ordering service database schema** (`ordering`) as data contract dependency.

### Architecture & Tech Stack

- **Framework:** .NET 10 Worker Service
- **Database:** PostgreSQL (Npgsql)
- **Messaging:** RabbitMQ (`eShop.EventBusRabbitMQ`)
- **Caching:** None configured

## Build & Deploy

```bash
# Build & publish (from repo root)
dotnet publish src/microservices/eShop.OrderProcessor/OrderProcessor/OrderProcessor.csproj -c Release -o ./publish

# Container build (from repo root)
docker build -t eshop/orderprocessor:latest -f src/microservices/eShop.OrderProcessor/Dockerfile src/microservices/eShop.OrderProcessor
```

### Environment Variables

| Variable                                 | Purpose                                        | Example                                                            | Required |
| ---------------------------------------- | ---------------------------------------------- | ------------------------------------------------------------------ | -------- |
| `ASPNETCORE_ENVIRONMENT`                 | Hosting environment                            | `Production`                                                       | Yes      |
| `ASPNETCORE_URLS`                        | Bind addresses                                 | `http://0.0.0.0:8080`                                              | Yes      |
| `ConnectionStrings__postgres`            | Ordering PostgreSQL connection                 | `Host=postgres;Database=OrderingDB;Username=postgres;Password=...` | Yes      |
| `ConnectionStrings__EventBus`            | RabbitMQ connection string                     | `amqp://guest:guest@rabbitmq:5672`                                 | Yes\*    |
| `EventBus__SubscriptionClientName`       | Event bus subscription/client identifier       | `OrderProcessor`                                                   | No       |
| `BackgroundTaskOptions__GracePeriodTime` | Minutes before submitted orders are confirmed  | `1`                                                                | No       |
| `BackgroundTaskOptions__CheckUpdateTime` | Polling interval in seconds                    | `30`                                                               | No       |
| `RabbitMQ__Host`                         | Broker host (alternative config shape)         | `rabbitmq`                                                         | No\*     |
| `RabbitMQ__Port`                         | Broker port (alternative config shape)         | `5672`                                                             | No\*     |
| `RabbitMQ__UserName`                     | Broker username (alternative config shape)     | `guest`                                                            | No\*     |
| `RabbitMQ__Password`                     | Broker password (alternative config shape)     | `guest`                                                            | No\*     |
| `RabbitMQ__VirtualHost`                  | Broker virtual host (alternative config shape) | `/`                                                                | No\*     |

\* RabbitMQ settings are required in practice for successful event publishing. You can provide either `ConnectionStrings__EventBus` or the `RabbitMQ__*` keys depending on deployment configuration.

## Testing

```bash
dotnet test src/microservices/eShop.OrderProcessor/eShop.OrderProcessor.sln
```
