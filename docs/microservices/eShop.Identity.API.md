# `eShop.Identity.API` Microservice

## Overview

### **Responsibility:**

- Host the **identity and authorization server** for the platform using **Duende IdentityServer** on **ASP.NET Core**.
- Handle **user registration, login, logout, consent, grants, and token issuance** with **ASP.NET Core Identity**.
- Provide the security authority for platform clients such as **WebApp**, **MAUI**, and the Swagger UIs for **Basket**, **Ordering**, and **Webhooks**.
- Persist identity data, client configuration, and operational state in **PostgreSQL** and apply migrations on startup.

### Dependence to other services

- **PostgreSQL** `identitydb` database for identity and IdentityServer storage.
- **WebApp**, **MAUI**, **Basket API**, **Ordering API**, and **Webhooks** client applications for OpenID Connect redirects and Swagger OAuth flows.
- **No RabbitMQ dependency** in the current implementation.

### Architecture & Tech Stack

- **Framework:** .NET 10 / ASP.NET Core
- **Authentication / Authorization:** Duende IdentityServer + ASP.NET Core Identity
- **Database:** PostgreSQL via Npgsql / Entity Framework Core
- **Messaging:** None
- **Caching:** None configured

## Build & Deploy

```bash
# Build & publish
dotnet publish src/microservices/eShop.Identity.API/Identity.API/Identity.API.csproj -c Release -o ./publish

# Container build (from repo root)
docker build -t eshop/identity-api:latest -f src/microservices/eShop.Identity.API/Dockerfile src/microservices/eShop.Identity.API
```

### Environment Variables

| Variable                        | Purpose                          | Example                                                            | Required |
| ------------------------------- | -------------------------------- | ------------------------------------------------------------------ | -------- |
| `ASPNETCORE_ENVIRONMENT`        | Hosting environment              | `Production`                                                       | Yes      |
| `ASPNETCORE_URLS`               | Bind addresses                   | `http://0.0.0.0:8080`                                              | Yes      |
| `ConnectionStrings__identitydb` | PostgreSQL connection            | `Host=sql-data;Database=identitydb;Username=postgres;Password=...` | Yes      |
| `MauiCallback`                  | MAUI client redirect URI         | `https://maui.example.com/callback`                                | Yes      |
| `WebAppClient`                  | WebApp client base URL           | `https://webapp.example.com`                                       | Yes      |
| `WebhooksWebClient`             | Webhooks web client URL          | `https://webhooks-web.example.com`                                 | Yes      |
| `BasketApiClient`               | Basket Swagger UI URL            | `https://basket-api.example.com`                                   | Yes      |
| `OrderingApiClient`             | Ordering Swagger UI URL          | `https://ordering-api.example.com`                                 | Yes      |
| `WebhooksApiClient`             | Webhooks API URL                 | `https://webhooks-api.example.com`                                 | Yes      |
| `TokenLifetimeMinutes`          | Access / identity token lifetime | `120`                                                              | No       |
| `PermanentTokenLifetimeDays`    | Refresh token lifetime           | `365`                                                              | No       |
| `UseCustomizationData`          | Seed custom UI/data flag         | `false`                                                            | No       |

## Testing

```bash
dotnet test src/microservices/eShop.Identity.API/eShop.Identity.API.sln
```
