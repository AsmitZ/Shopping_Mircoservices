using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

using ApiType = SD.ApiType;

public class CouponService : ICouponService
{
    private readonly IBaseService _baseService;
    public CouponService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDto?> GetAllCouponsAsync()
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.CouponApiBase + "api/coupons"
        });
    }

    public async Task<ResponseDto?> GetCouponAsync(string couponCode)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.CouponApiBase + "api/coupons/" + couponCode
        });
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int couponId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.CouponApiBase + "api/coupons/" + couponId
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int couponId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.DELETE,
            Url = SD.CouponApiBase + "api/coupons/" + couponId
        });
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.POST,
            Url = SD.CouponApiBase + "api/coupons",
            Data = couponDto
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.PUT,
            Url = SD.CouponApiBase + "api/coupons",
            Data = couponDto
        });
    }
}