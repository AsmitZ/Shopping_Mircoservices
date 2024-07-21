using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface ICartService
{
    Task<ResponseDto?> GetCartByUserIdAsync(string userId);

    Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
    
    Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);

    Task<ResponseDto?> ApplyCouponAsync(string userId, string couponsCode);

    Task<ResponseDto?> RemoveCouponAsync(string userId);
}