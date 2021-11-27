using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Orders.PubSub;

public class OrderPublisher : IOrderPublisher
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ISubscriber _sub;
    private readonly ILogger<OrderPublisher> _log;

    public OrderPublisher(ILogger<OrderPublisher> log)
    {
        _log = log;
        _redis = ConnectionMultiplexer.Connect(Constants.ConnectionString);
        _sub = _redis.GetSubscriber();
    }

    public async Task PublishOrder(string crustId, IEnumerable<string> toppingIds, DateTimeOffset time)
    {
        var message = new OrderMessage
        {
            CrustId = crustId,
            ToppingIds = toppingIds.ToArray(),
            Time = time
        }.ToBytes();
        await _sub.PublishAsync("orders", message);
    }

    public void Dispose()
    {
        _redis.Dispose();
    }
}