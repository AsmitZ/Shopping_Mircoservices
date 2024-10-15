using System.ComponentModel.DataAnnotations;
using Mango.Web.Utilities;

namespace Mango.Web.Models;

public class ProductDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string? ImageURL { get; set; }
    public string? ImageLocalPath { get; set; }
    public string Category { get; set; }
    [Range(0, 100)] public int Count { get; set; } = 1;

    [AllowExtensions([".jpg", ".png", ".jpeg"])]
    [MaxFileSize(1 * 1024 * 1024)]
    public IFormFile? Image { get; set; }
}