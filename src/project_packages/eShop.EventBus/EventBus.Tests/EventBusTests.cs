using System.Text.Json;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using eShop.EventBus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace eShop.EventBus.Tests;

public class EventBusTests
{
    [Fact]
    public void IntegrationEvent_initializes_with_new_id_and_creation_time()
    {
        var before = DateTime.UtcNow;

        var integrationEvent = new SampleIntegrationEvent();

        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, integrationEvent.Id);
        Assert.InRange(integrationEvent.CreationDate, before, after);
    }

    [Fact]
    public void GetGenericTypeName_returns_simple_name_for_non_generic_type()
    {
        var typeName = typeof(SampleIntegrationEvent).GetGenericTypeName();

        Assert.Equal(nameof(SampleIntegrationEvent), typeName);
    }

    [Fact]
    public void GetGenericTypeName_returns_generic_name_for_generic_type()
    {
        var typeName = typeof(List<int>).GetGenericTypeName();

        Assert.Equal("List<Int32>", typeName);
    }

    [Fact]
    public void ConfigureJsonOptions_applies_custom_serializer_settings()
    {
        var builder = new TestEventBusBuilder();

        builder.ConfigureJsonOptions(options => options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        var serviceProvider = builder.Services.AddOptions().BuildServiceProvider();
        var subscriptionInfo = serviceProvider.GetRequiredService<IOptions<EventBusSubscriptionInfo>>().Value;

        Assert.Same(JsonNamingPolicy.CamelCase, subscriptionInfo.JsonSerializerOptions.PropertyNamingPolicy);
    }

    [Fact]
    public void AddSubscription_registers_handler_and_event_type_mapping()
    {
        var builder = new TestEventBusBuilder();

        builder.AddSubscription<SampleIntegrationEvent, SampleIntegrationEventHandler>();

        var serviceProvider = builder.Services.AddOptions().BuildServiceProvider();

        var subscriptionInfo = serviceProvider.GetRequiredService<IOptions<EventBusSubscriptionInfo>>().Value;
        var handler = serviceProvider.GetRequiredKeyedService<IIntegrationEventHandler>(typeof(SampleIntegrationEvent));

        Assert.True(subscriptionInfo.EventTypes.TryGetValue(nameof(SampleIntegrationEvent), out var mappedType));
        Assert.Equal(typeof(SampleIntegrationEvent), mappedType);
        Assert.IsType<SampleIntegrationEventHandler>(handler);
    }

    private sealed class TestEventBusBuilder : IEventBusBuilder
    {
        public IServiceCollection Services { get; } = new ServiceCollection();
    }

    private sealed record SampleIntegrationEvent : IntegrationEvent;

    private sealed class SampleIntegrationEventHandler : IIntegrationEventHandler<SampleIntegrationEvent>
    {
        public Task Handle(SampleIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}