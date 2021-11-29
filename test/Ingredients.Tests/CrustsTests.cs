using System.Threading.Tasks;
using Ingredients.Protos;
using Xunit;

namespace Ingredients.Tests;

public class CrustsTests : IClassFixture<IngredientsApplicationFactory>
{
    private readonly IngredientsApplicationFactory _factory;

    public CrustsTests(IngredientsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetsCrusts()
    {
        var client = _factory.CreateGrpcClient();
        var response = await client.GetCrustsAsync(new GetCrustsRequest());
        
        Assert.Collection(response.Crusts,
            t =>
            {
                Assert.Equal("thin", t.Id);
            });
    }
}