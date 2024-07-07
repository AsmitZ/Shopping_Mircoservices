using Mango.Services.CartAPI.Models.Dto;

namespace Mango.Services.CartAPI.Services.IServices;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProducts();
}