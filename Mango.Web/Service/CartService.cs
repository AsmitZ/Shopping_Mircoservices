using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

using ApiType = SD.ApiType;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;
    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.ShoppingCartApiBase + "api/cart/GetCart/" + userId
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartApiBase + "api/cart/CartUpsert"
        });
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Data = cartDetailsId,
            Url = SD.ShoppingCartApiBase + "api/cart/RemoveCart"
        });
    }

    public async Task<ResponseDto?> ApplyCouponAsync(string userId, string couponCode)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.PUT,
            Data = couponCode,
            Url = SD.ShoppingCartApiBase + "api/cart/ApplyCoupon/" + userId
        });
    }
}