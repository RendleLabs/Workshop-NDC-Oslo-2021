namespace Frontend.Models;

public class CrustViewModel
{
    public CrustViewModel()
    {
            
    }
    public CrustViewModel(string id, string name, int size, decimal price)
    {
        Id = id;
        Name = name;
        Size = size;
        Price = price;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public decimal Price { get; set; }
}