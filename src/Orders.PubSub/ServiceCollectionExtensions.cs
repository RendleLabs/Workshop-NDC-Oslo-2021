using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Orders.PubSub;

public static class ServiceCollectionExtensions
{
    public static void AddOrderPubSub(this IServiceCollection services, IConnectionMultiplexer connectionMultiplexer)
    {
        var orderMessages = new OrderMessages();
        services.AddSingleton(orderMessages);
        services.AddSingleton<IOrderMessages>(orderMessages);
        services.AddSingleton<IOrderPublisher, OrderPublisher>();
        services.AddHostedService<OrderSubscriber>();
        services.AddSingleton(connectionMultiplexer);
    }
}