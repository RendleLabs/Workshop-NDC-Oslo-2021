using Orders.Protos;
using ShopConsole;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddGrpcClient<OrderService.OrderServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5005");
        });
    })
    .Build();

await host.RunAsync();
