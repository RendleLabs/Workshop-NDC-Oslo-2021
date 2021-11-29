using Grpc.Core;
using Orders.Protos;

namespace ShopConsole;

public class Worker : BackgroundService
{
    private readonly OrderService.OrderServiceClient _orders;
    private readonly ILogger<Worker> _logger;

    public Worker(OrderService.OrderServiceClient orders, ILogger<Worker> logger)
    {
        _orders = orders;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var call = _orders.Subscribe(new SubscribeRequest());

                await foreach (var notification in call.ResponseStream.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine($"Order received: {notification.CrustId}");
                    foreach (var toppingId in notification.ToppingIds)
                    {
                        Console.WriteLine($"    {toppingId}");
                    }

                    var dueBy = notification.Time.ToDateTimeOffset().AddHours(0.5);
                    Console.WriteLine($"DUE BY: {dueBy:t}");
                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: {Message}", e.Message);
                
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
