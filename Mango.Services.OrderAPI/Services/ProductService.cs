using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _client;

    public ProductService(IHttpClientFactory client)
    {
        _client = client;
    }

    public async Task<IEnumerable<ProductDto>> GetProducts()
    {
        var client = _client.CreateClient("ProductAPI");
        var response = await client.GetAsync("api/products");
        var apiContent = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync());
        if (apiContent.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(apiContent.Result.ToString());
            return result;
        }

        return new List<ProductDto>();
    }
}