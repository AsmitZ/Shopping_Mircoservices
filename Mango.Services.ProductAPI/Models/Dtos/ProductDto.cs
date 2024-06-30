namespace Mango.Services.ProductAPI.Models.Dtos;

public class ProductDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string ImageURL { get; set; }
    public string Category { get; set; }
}