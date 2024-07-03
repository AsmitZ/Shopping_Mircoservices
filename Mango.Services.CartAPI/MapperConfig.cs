using AutoMapper;
using Mango.Services.CartAPI.Models;
using Mango.Services.CartAPI.Models.Dto;

namespace Mango.Services.CartAPI;

public class MapperConfig
{
    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            cfg.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        });
    }
}