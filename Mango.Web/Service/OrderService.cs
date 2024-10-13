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
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Url = SD.OrderApiBase + "api/order/",
            Data = cartDto
        });

        return responseDto;
    }

    public async Task<ResponseDto?> CreateSessionAsync(PaymentRequestDto request)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Url = SD.OrderApiBase + "api/order/session",
            Data = request
        });

        return responseDto;
    }
}