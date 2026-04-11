# eShop.EventBus

`eShop.EventBus` provides the event bus abstractions used by eShop services for integration events.

## What it includes

- `IntegrationEvent` base record with an automatically generated `Id` and `CreationDate`
- `IEventBus` for publishing integration events
- `IIntegrationEventHandler<T>` for handling typed integration events
- `IEventBusBuilder` extension methods for registration and JSON configuration

## Getting started

Create your integration event by inheriting from `IntegrationEvent`:

```csharp
public sealed record OrderCreatedIntegrationEvent(Guid OrderId) : IntegrationEvent;
```

Create a handler for the event:

```csharp
using eShop.EventBus.Abstractions;

public sealed class OrderCreatedIntegrationEventHandler
    : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        return Task.CompletedTask;
    }
}
```

Register the event and handler through the event bus builder:

```csharp
builder
    .AddSubscription<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>()
    .ConfigureJsonOptions(options =>
    {
        options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
```

## Notes

- The package targets `net10.0` and is marked AOT-compatible.
- Event types are tracked by name for subscription lookup and serialization metadata.
