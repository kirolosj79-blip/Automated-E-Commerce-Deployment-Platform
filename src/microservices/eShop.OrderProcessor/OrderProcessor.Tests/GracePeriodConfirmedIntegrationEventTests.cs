using eShop.OrderProcessor.Events;

namespace eShop.OrderProcessor.Tests;

public class GracePeriodConfirmedIntegrationEventTests
{
    [Fact]
    public void Constructor_sets_OrderId()
    {
        var evt = new GracePeriodConfirmedIntegrationEvent(42);

        Assert.Equal(42, evt.OrderId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void Constructor_preserves_order_id_value(int orderId)
    {
        var evt = new GracePeriodConfirmedIntegrationEvent(orderId);

        Assert.Equal(orderId, evt.OrderId);
    }

    [Fact]
    public void Id_is_assigned_by_base_integration_event()
    {
        var evt = new GracePeriodConfirmedIntegrationEvent(1);

        Assert.NotEqual(Guid.Empty, evt.Id);
    }
}
