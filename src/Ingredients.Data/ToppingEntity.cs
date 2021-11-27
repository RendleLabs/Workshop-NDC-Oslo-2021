using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;

namespace Ingredients.Data;

public class ToppingEntity : ITableEntity
{
    public ToppingEntity()
    {
        PartitionKey = "toppings";
    }
        
    public ToppingEntity(string id, string name, double price, int stockCount) : this()
    {
        Id = id;
        Name = name;
        Price = price;
        StockCount = stockCount;
    }

    [IgnoreDataMember]
    public string Id
    {
        get => RowKey;
        set => RowKey = value;
    }
    public string Name { get; set; }
    public double Price { get; set; }
    public int StockCount { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}