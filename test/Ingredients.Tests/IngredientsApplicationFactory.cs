using System.Collections.Generic;
using System.Threading;
using GrpcTestHelper;
using Ingredients.Data;
using Ingredients.Protos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Ingredients.Tests;

public class IngredientsApplicationFactory : WebApplicationFactory<Marker>
{
    public IngredientsService.IngredientsServiceClient CreateGrpcClient()
    {
        var channel = this.CreateGrpcChannel();
        return new IngredientsService.IngredientsServiceClient(channel);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IToppingData>();

            var toppings = new List<ToppingEntity>
            {
                new("cheese", "Cheese", 0.5d, 10)
            };

            var toppingSub = Substitute.For<IToppingData>();
            toppingSub.GetAsync(Arg.Any<CancellationToken>())
                .Returns(toppings);

            services.AddSingleton(toppingSub);
        });
        
        base.ConfigureWebHost(builder);
    }
}