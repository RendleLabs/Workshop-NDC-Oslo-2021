using Grpc.Health.V1;
using Grpc.HealthCheck;
using Ingredients.Data;

namespace Ingredients;

public class HealthCheckService : BackgroundService
{
    private readonly IToppingData _data;
    private readonly HealthServiceImpl _healthServiceImpl;

    public HealthCheckService(IToppingData data, HealthServiceImpl healthServiceImpl)
    {
        _data = data;
        _healthServiceImpl = healthServiceImpl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var _ = await _data.GetAsync(stoppingToken);
                _healthServiceImpl.SetStatus("Ingredients", HealthCheckResponse.Types.ServingStatus.Serving);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                _healthServiceImpl.SetStatus("Ingredients", HealthCheckResponse.Types.ServingStatus.NotServing);
            }

        }
    }
}