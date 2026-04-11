using eShop.EventBus.Abstractions;
using eShop.OrderProcessor.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;

namespace eShop.OrderProcessor.Tests;

public class GracePeriodManagerServiceTests
{
    private static NpgsqlDataSource CreateTestDataSource() =>
        NpgsqlDataSource.Create("Host=127.0.0.1;Port=1;Database=test;Username=test;Password=test");

    [Fact]
    public void Constructor_throws_when_options_is_null()
    {
        using var dataSource = CreateTestDataSource();
        Assert.Throws<ArgumentNullException>(() => new GracePeriodManagerService(
            null!,
            Mock.Of<IEventBus>(),
            NullLogger<GracePeriodManagerService>.Instance,
            dataSource));
    }

    [Fact]
    public void Constructor_accepts_valid_dependencies()
    {
        var options = Options.Create(new BackgroundTaskOptions
        {
            GracePeriodTime = 15,
            CheckUpdateTime = 5,
        });

        using var dataSource = CreateTestDataSource();
        var service = new GracePeriodManagerService(
            options,
            Mock.Of<IEventBus>(),
            NullLogger<GracePeriodManagerService>.Instance,
            dataSource);

        Assert.NotNull(service);
    }
}
