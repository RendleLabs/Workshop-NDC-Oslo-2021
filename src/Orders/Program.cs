
using System;
using Ingredients.Protos;
using JaegerTracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.PubSub;
using Orders.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connectionMultiplexer = ConnectionMultiplexer.Connect(Constants.ConnectionString);
builder.Services.AddOrderPubSub(connectionMultiplexer);
builder.AddJaegerTracing(connectionMultiplexer, "Orders");

builder.Services.AddGrpc();


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
