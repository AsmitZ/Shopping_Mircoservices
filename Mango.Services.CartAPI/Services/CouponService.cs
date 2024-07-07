using AutoMapper;
using Mango.Services.CartAPI.Models.Dto;
using Mango.Services.CartAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.CartAPI.Services;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _client;
    public CouponService(IHttpClientFactory client)
    {
        _client = client;
    }
    
    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var client = _client.CreateClient("CouponAPI");
        var response = await client.GetAsync("api/coupons/" + couponCode);
        var apiContent = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync());
        if (apiContent is null)
        {
            Console.WriteLine("Unable to get coupon from CouponAPI");
            return new CouponDto();
        }
        if (apiContent.IsSuccess)
        {
            var result = JsonConvert.DeserializeObject<CouponDto>(apiContent.Result.ToString());
            return result;
        }

        return new CouponDto();
    }
}