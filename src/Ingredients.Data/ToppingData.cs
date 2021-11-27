using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Ingredients.Data;

public class ToppingData : IToppingData
{
    private readonly ILogger<ToppingData> _log;
    private const string TableName = "toppings";
    private readonly TableClient _client;

    public ToppingData(ILogger<ToppingData> log)
    {
        _log = log;
        _client = new TableClient(Constants.StorageConnectionString, TableName);
    }

    public async Task<List<ToppingEntity>> GetAsync(CancellationToken token = default)
    {
        try
        {
            return await _client.QueryAsync<ToppingEntity>(cancellationToken: token).ToListAsync(token);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error reading data.");
            throw;
        }
    }

    public async Task AddAsync(string id, string name, double price, int stockCount)
    {
        try
        {
            var entity = new ToppingEntity(id, name, price, stockCount);
            await _client.AddEntityAsync(entity);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error inserting data.");
            throw;
        }
    }

    public async Task DecrementStockAsync(string id, CancellationToken token = default)
    {
        for (int i = 0; i < 100; i++)
        {
            ToppingEntity entity;
            try
            {
                var response = await _client.GetEntityAsync<ToppingEntity>("topping", id, cancellationToken: token);
                entity = response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                _log.LogError(ex, "Data not found");
                return;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error retrieving data.");
                throw;
            }
            if (entity.StockCount == 0) return;
            entity.StockCount--;
            try
            {
                await _client.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace, token);
                break;
            }
            catch (RequestFailedException ex) when (ex.Status == 412)
            {
                _log.LogInformation("Conflict updating entity, retrying.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating data.");
                throw;
            }
        }
    }
}