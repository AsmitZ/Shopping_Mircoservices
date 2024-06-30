using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface ICouponService
{
    Task<ResponseDto?> GetAllCouponsAsync();

    Task<ResponseDto?> GetCouponAsync(string couponCode);
    
    Task<ResponseDto?> GetCouponByIdAsync(int couponId);

    Task<ResponseDto?> DeleteCouponAsync(int couponId);

    Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
    
    Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
}