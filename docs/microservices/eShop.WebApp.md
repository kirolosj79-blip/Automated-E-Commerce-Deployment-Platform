# `eShop.WebApp` Microservice

## Overview

### **Responsibility:**

- Host the **customer-facing store UI** as an **ASP.NET Core Blazor Server** app (interactive server components, Razor, static files).
- Act as a **Backend-for-Frontend (BFF)**: call **Catalog** and **Ordering** over HTTP (with API versioning and auth token propagation), and **Basket** over **gRPC**.
- **Authenticate users** via **OpenID Connect** against the eShop **Identity** authority (cookie session + OIDC challenge).
- **Proxy product images** from the catalog API through **YARP** (`/product-images/{id}` → catalog item picture endpoint).
- **Subscribe to RabbitMQ integration events** for **order status** changes and push updates to signed-in users via `OrderStatusNotificationService` (Blazor UI notifications).

### Dependence to other services

- **Identity** (OpenID Connect authority — discovery, authorization code flow, sign-out).
- **Catalog.API** — HTTP JSON (catalog items, brands, types, semantic search); **YARP** forwarder for images.
- **Ordering.API** — HTTP JSON (`/api/Orders/`).
- **Basket.API** — **gRPC** (`Basket.BasketClient`: get/update/delete basket).
- **RabbitMQ** — **consumer** for order-status integration events (no direct database in this service).

### Architecture & Tech Stack

- **Framework:** .NET 10 / ASP.NET Core (Blazor Server, YARP forwarding, cookie + OIDC auth).
- **Database:** None in-process (no connection string for an app-owned DB; persistence lives in backend services).
- **Messaging:** RabbitMQ (via `eShop.EventBusRabbitMQ` / `AddRabbitMqEventBus`).
- **Caching:** None configured in this codebase.

---

## Build & Deploy

```bash
# Build & publish (eShop.WebApp workspace — project under WebApp/)
dotnet publish WebApp/WebApp.csproj -c Release -o ./publish

# Build & publish (full dotnet/eShop repo layout)
dotnet publish src/eShop.WebApp/eShop.WebApp.csproj -c Release -o ./publish

# Run published output
dotnet ./publish/WebApp.dll
```

```bash
# Container build (typical full eShop repo — Dockerfile path may vary by branch)
docker build -t eshop/webapp:latest -f src/eShop.WebApp/Dockerfile .

# Kubernetes (illustrative — align names/labels with your manifests)
kubectl apply -f deploy/webapp.yaml
# Ensure env vars / ConfigMaps supply IdentityUrl, CallBackUrl, *ServiceUrl, ConnectionStrings__EventBus, etc.
```

If you only have an extracted solution and **no Dockerfile**, use a multi-stage `Dockerfile` that runs `dotnet publish` on `WebApp/WebApp.csproj` and sets `ASPNETCORE_URLS` (for example `http://0.0.0.0:8080`).

---

### Environment Variables

.NET binds hierarchical configuration to environment variables using `__` (e.g. `EventBus__SubscriptionClientName`). Keys below are those used or implied by the **eShop.WebApp** codebase plus **shared** hosting/telemetry patterns from `AddServiceDefaults()` / `MapDefaultEndpoints()` (as in upstream eShop’s ServiceDefaults: health endpoints, optional OTLP when the endpoint is set).

| Variable | Purpose | Example | Required |
| -------- | ------- | ------- | -------- |
| `ASPNETCORE_ENVIRONMENT` | Hosting environment (`Development`, `Production`, …) | `Production` | Yes |
| `ASPNETCORE_URLS` | Kestrel bind URLs | `http://0.0.0.0:8080` | Yes (typical in containers) |
| `AllowedHosts` | Host header allow list | `*` | No (default in `appsettings.json`) |
| `IdentityUrl` | OIDC authority (Identity service) | `http://identity-api:8080` | Yes |
| `CallBackUrl` | Base URL for OIDC `SignedOutRedirectUri` / app callback alignment | `https://store.example.com` | Yes |
| `CatalogServiceUrl` | Catalog API base URI (HTTP client + YARP forwarder target) | `http://catalog-api:8080` | Yes |
| `OrderServiceUrl` | Ordering API base URI | `http://ordering-api:8080` | Yes |
| `BasketServiceUrl` | Basket gRPC server address | `http://basket-api:8080` | Yes |
| `SessionCookieLifetimeMinutes` | Auth cookie lifetime (minutes) | `60` | No |
| `EventBus__SubscriptionClientName` | RabbitMQ consumer subscription name | `Ordering.webapp` | No* |
| `ConnectionStrings__EventBus` | AMQP connection string for the event bus | `amqp://guest:pass@rabbitmq:5672/` | Yes* |
| `RabbitMQ__Host` | Broker host (when using discrete RabbitMQ options) | `rabbitmq` | No* |
| `RabbitMQ__Port` | Broker port | `5672` | No* |
| `RabbitMQ__UserName` | Broker user | `guest` | No* |
| `RabbitMQ__Password` | Broker password | `guest` | No* |
| `RabbitMQ__VirtualHost` | Virtual host | `/` | No* |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry OTLP exporter (enabled when set, per ServiceDefaults pattern) | `http://otel-collector:4317` | No |
| `Logging__LogLevel__Default` | Default log level | `Information` | No |
| `Logging__LogLevel__Microsoft.AspNetCore` | ASP.NET Core log level | `Warning` | No |

\* **Event bus:** Required for **live order-status notifications** via RabbitMQ; if unset or disconnected, the UI may still work but handlers will not receive integration events. **`ConnectionStrings__EventBus`** appears in `appsettings.Development.json`; **`RabbitMQ__*`** appears there as an alternate shape—confirm which your `eShop.EventBusRabbitMQ` build reads, or supply **`ConnectionStrings__EventBus`** for parity with Development.

**Not environment-driven in current code:** OpenID Connect `ClientId` (`webapp`) and `ClientSecret` (`secret`) are **hardcoded** in `AddAuthenticationServices`; production setups usually replace these via **code/config extension** or **user secrets** (`UserSecretsId` on the project), not via variables listed above.

**Service discovery:** HTTP clients use **`AddServiceDiscovery()`**; `*ServiceUrl` values can be resolvable hostnames (e.g. Kubernetes DNS) or discovery URIs supported by your platform (e.g. `https+http://catalog-api` when using .NET service discovery).

---

## Testing

```bash
# From solution root (eShop.WebApp workspace)
dotnet test WebApp.Tests/WebApp.Tests.csproj

# Or test entire solution
dotnet test
```
