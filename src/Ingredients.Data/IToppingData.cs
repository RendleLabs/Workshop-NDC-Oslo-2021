namespace Ingredients.Data;

public interface IToppingData
{
    Task<List<ToppingEntity>> GetAsync(CancellationToken token = default);
    Task DecrementStockAsync(string id, CancellationToken token = default);
}