using Grpc.HealthCheck;
using Ingredients;
using Ingredients.Data;
using Ingredients.Services;
using JaegerTracing;

var builder = WebApplication.CreateBuilder(args);

builder.AddJaegerTracing();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<IToppingData, ToppingData>();
builder.Services.AddSingleton<ICrustData, CrustData>();

builder.Services.AddSingleton<HealthServiceImpl>();
builder.Services.AddHostedService<HealthCheckService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<IngredientsImpl>();
app.MapGrpcService<HealthServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
