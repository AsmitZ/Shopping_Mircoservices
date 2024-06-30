using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dtos;

namespace Mango.Services.CouponAPI;

public class MapperConfig
{
    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Coupon, CouponDto>();
            cfg.CreateMap<CouponDto, Coupon>();
        });
    }
}