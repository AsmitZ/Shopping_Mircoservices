using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

using ApiType = SD.ApiType;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;

    public OrderService(IBaseService baseService)
    {
        ArgumentNullException.ThrowIfNull(baseService);
        _baseService = baseService;
    }

    public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Url = SD.OrderApiBase + "api/order/",
            Data = cartDto
        });
    }
}