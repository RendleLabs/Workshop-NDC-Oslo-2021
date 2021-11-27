using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Ingredients.Data;

public class CrustData : ICrustData
{
    private readonly ILogger<CrustData> _log;
    private const string TableName = "crusts";
    private readonly TableClient _client;

    public CrustData(ILogger<CrustData> log)
    {
        _log = log;
        _client = new TableClient(Constants.StorageConnectionString, TableName);
    }

    public async Task<List<CrustEntity>> GetAsync(CancellationToken token = default)
    {
        try
        {
            return await _client.QueryAsync<CrustEntity>().ToListAsync(token);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error reading data.");
            throw;
        }
    }

    public async Task AddAsync(string id, string name, int size, double price, int stockCount)
    {
        try
        {
            var entity = new CrustEntity(id, name, size, price, stockCount);
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
            var response = await _client.GetEntityAsync<CrustEntity>("crust", id, cancellationToken: token);
            var entity = response.Value;
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
            }
        }
    }
}