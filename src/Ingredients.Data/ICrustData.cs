namespace Ingredients.Data;

public interface ICrustData
{
    Task<List<CrustEntity>> GetAsync(CancellationToken token = default);
    Task DecrementStockAsync(string id, CancellationToken token = default);
}