using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;

namespace Ingredients.Data;

public class CrustEntity : ITableEntity
{
    public CrustEntity()
    {
        PartitionKey = "crust";
    }
        
    public CrustEntity(string id, string name, int size, double price, int stockCount) : this()
    {
        Id = id;
        Name = name;
        Size = size;
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
    public int Size { get; set; }
    public double Price { get; set; }
    public int StockCount { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}