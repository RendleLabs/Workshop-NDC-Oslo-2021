using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orders.PubSub;

public interface IOrderPublisher : IDisposable
{
    Task PublishOrder(string crustId, IEnumerable<string> toppingIds, DateTimeOffset time);
}