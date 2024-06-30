using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.Models;

public class Product
{
    [Key]
    public long ProductId { get; set; }
    [Required]
    public string Name { get; set; }
    [Range(1, 1000)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string ImageURL { get; set; }
    public string Category { get; set; }
}