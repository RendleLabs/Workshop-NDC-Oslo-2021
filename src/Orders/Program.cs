
using Ingredients.Protos;
using Orders.PubSub;
using Orders.Services;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.AddJaegerTracing();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddOrderPubSub();

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var uri = configuration.GetServiceUri("Ingredients");
    options.Address = uri ?? new Uri("https://localhost:5003");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<OrdersImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
