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
        ArgumentNullException.ThrowIfNull(baseService);
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.ShoppingCartApiBase + "api/carts/" + userId
        });
        return responseDto;
    }

    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.PUT,
            Data = cartDto,
            Url = SD.ShoppingCartApiBase + "api/carts"
        });
        return responseDto;
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.DELETE,
            Url = SD.ShoppingCartApiBase + "api/carts/" + cartDetailsId
        });
        return responseDto;
    }

    public async Task<ResponseDto?> ApplyCouponAsync(string userId, string couponCode)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.PUT,
            Url = SD.ShoppingCartApiBase + "api/carts/coupon/apply/" + userId + "/" + couponCode
        });
        return responseDto;
    }

    public async Task<ResponseDto?> RemoveCouponAsync(string userId)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.PUT,
            Url = SD.ShoppingCartApiBase + "api/carts/coupon/remove/" + userId
        });
        return responseDto;
    }

    public async Task<ResponseDto?> EmailCartAsync(CartDto cartDto)
    {
        var responseDto = await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartApiBase + "api/carts/email"
        });
        return responseDto;
    }
}