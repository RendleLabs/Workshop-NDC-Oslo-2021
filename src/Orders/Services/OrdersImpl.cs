using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ingredients.Protos;
using Orders.Protos;

namespace Orders.Services;

public class OrdersImpl : OrderService.OrderServiceBase
{
    private readonly IngredientsService.IngredientsServiceClient _ingredients;
    private readonly ILogger<OrdersImpl> _logger;

    public OrdersImpl(IngredientsService.IngredientsServiceClient ingredients, ILogger<OrdersImpl> logger)
    {
        _ingredients = ingredients;
        _logger = logger;
    }

    public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        var decrementToppingsRequest = new DecrementToppingsRequest
        {
            ToppingIds = {request.ToppingIds}
        };
        await _ingredients.DecrementToppingsAsync(decrementToppingsRequest);

        var decrementCrustsRequest = new DecrementCrustsRequest
        {
            CrustId = request.CrustId
        };
        await _ingredients.DecrementCrustsAsync(decrementCrustsRequest);

        var now = DateTimeOffset.UtcNow;

        return new PlaceOrderResponse
        {
            Time = now.ToTimestamp()
        };
    }
}
