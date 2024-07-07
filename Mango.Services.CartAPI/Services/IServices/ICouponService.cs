using Mango.Services.CartAPI.Models.Dto;

namespace Mango.Services.CartAPI.Services.IServices;

public interface ICouponService
{
    Task<CouponDto> GetCoupon(string couponCode);
}