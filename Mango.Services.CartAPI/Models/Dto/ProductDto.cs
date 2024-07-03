namespace Mango.Services.CartAPI.Models.Dto;

public class ProductDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string ImageURL { get; set; }
    public string Category { get; set; }
}