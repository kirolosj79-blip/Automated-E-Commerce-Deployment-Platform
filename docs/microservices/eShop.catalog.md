# eShop.Catalog.API Microservice

## Overview

### Responsibility:
The Catalog microservice is responsible for managing product catalog data within the platform.

It handles product creation, updates, deletion, and retrieval, including support for product categories, pricing, stock information, and product images.

It serves as the central service for all product-related operations used by WebApp, MAUI, Basket, and Ordering services.

It persists catalog data in PostgreSQL and applies database migrations on startup using Entity Framework Core.

---

## Dependence to other services

- PostgreSQL `catalogdb` database for storing product catalog data.
- WebApp and MAUI clients for displaying products and categories.
- Basket API for adding products to shopping cart.
- Ordering API for referencing product data during order creation.

No direct dependency on IdentityServer or RabbitMQ in the current implementation.

---

## Architecture & Tech Stack

- **Framework:** .NET 10 / ASP.NET Core Web API  
- **ORM:** Entity Framework Core  
- **Database:** PostgreSQL (Npgsql provider)  
- **Architecture Style:** Clean / Layered Architecture  
- **Messaging:** None  
- **Caching:** None configured  

---
## Build & Deploy

```bash
# Build & publish
dotnet publish src/microservices/eShop.Catalog.API/eShop.Catalog.API.csproj -c Release -o ./publish
# Container build (from repo root)
docker build -t eshop/catalog-api:latest -f src/microservices/eShop.Catalog.API/Dockerfile src/microservices/eShop.Catalog.API
```
## Environment Variables


| Variable                     | Purpose                  | Example                                                         | Required |
| ---------------------------- | ------------------------ | --------------------------------------------------------------- | -------- |
| ASPNETCORE_ENVIRONMENT       | Hosting environment      | Production                                                      | Yes      |
| ASPNETCORE_URLS              | Bind addresses           | [http://0.0.0.0:8080](http://0.0.0.0:8080)                      | Yes      |
| ConnectionStrings__catalogdb | PostgreSQL connection    | Host=sql-data;Database=catalogdb;Username=postgres;Password=... | Yes      |
| UseCustomizationData         | Seed sample catalog data | false                                                           | No       |
| EnableSwagger                | Enable API documentation | true                                                            | No       |

## Testing

dotnet test src/microservices/eShop.Catalog.API/eShop.Catalog.API.sln
