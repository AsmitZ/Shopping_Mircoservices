using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class ProductService(IBaseService baseService) : IProductService
{
    public async Task<ResponseDto?> GetAllProductsAsync()
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ProductApiBase + "api/products",
        });
    }

    public async Task<ResponseDto?> GetProductByIdAsync(long productId)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ProductApiBase + "api/products/" + productId,
        });
    }

    public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ProductApiBase + "api/products",
            Data = productDto
        });
    }

    public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.PUT,
            Url = SD.ProductApiBase + "api/products",
            Data = productDto
        });
    }

    public async Task<ResponseDto?> DeleteProductAsync(long productId)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.DELETE,
            Url = SD.ProductApiBase + "api/products/" + productId,
        });
    }
}