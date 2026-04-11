using Npgsql;
using System.Text.Json.Serialization;
using eShop.OrderProcessor.Events;

namespace eShop.OrderProcessor.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRabbitMqEventBus(builder.Configuration)
               .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));

        builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(
            builder.Configuration["ConnectionStrings:postgres"]
            ?? throw new InvalidOperationException("Missing ConnectionStrings:postgres configuration.")));

        builder.Services.AddOptions<BackgroundTaskOptions>()
            .BindConfiguration(nameof(BackgroundTaskOptions));

        builder.Services.AddHostedService<GracePeriodManagerService>();
    }
}

[JsonSerializable(typeof(GracePeriodConfirmedIntegrationEvent))]
partial class IntegrationEventContext : JsonSerializerContext
{

}
