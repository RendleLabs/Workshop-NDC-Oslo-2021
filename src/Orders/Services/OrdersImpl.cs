using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ingredients.Protos;
using Orders.Protos;
using Orders.PubSub;

namespace Orders.Services;

public class OrdersImpl : OrderService.OrderServiceBase
{
    private readonly IngredientsService.IngredientsServiceClient _ingredients;
    private readonly IOrderPublisher _orderPublisher;
    private readonly IOrderMessages _orderMessages;
    private readonly ILogger<OrdersImpl> _logger;

    public OrdersImpl(IngredientsService.IngredientsServiceClient ingredients,
        IOrderPublisher orderPublisher,
        IOrderMessages orderMessages,
        ILogger<OrdersImpl> logger)
    {
        _ingredients = ingredients;
        _orderPublisher = orderPublisher;
        _orderMessages = orderMessages;
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

        await _orderPublisher.PublishOrder(request.CrustId, request.ToppingIds, now);

        return new PlaceOrderResponse
        {
            Time = now.ToTimestamp()
        };
    }

    public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<OrderNotification> responseStream, ServerCallContext context)
    {
        var token = context.CancellationToken;

        while (!token.IsCancellationRequested)
        {
            try
            {
                var message = await _orderMessages.ReadAsync(token);
                var notification = new OrderNotification
                {
                    CrustId = message.CrustId,
                    ToppingIds = {message.ToppingIds},
                    Time = message.Time.ToTimestamp()
                };

                try
                {
                    await responseStream.WriteAsync(notification);
                }
                catch
                {
                    await _orderPublisher.PublishOrder(message.CrustId, message.ToppingIds, message.Time);
                    throw;
                }
                
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
