using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI;

public static class MapperConfig
{
    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(des => des.CartHeaderId, opt => opt.MapFrom(des => des.OrderHeaderId))
                .ForMember(des => des.CartTotal, opt => opt.MapFrom(des => des.OrderTotal))
                .ReverseMap();
            cfg.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(des => des.OrderHeaderId, opt => opt.MapFrom(src => src.CartHeaderId))
                .ForMember(des => des.OrderDetailsId, opt => opt.MapFrom(src => src.CartDetailsId))
                .ForMember(des => des.ProductName, opt => opt.MapFrom(des => des.Product.Name))
                .ForMember(des => des.ProductPrice, opt => opt.MapFrom(des => des.Product.Price))
                .ReverseMap();
            cfg.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ReverseMap();
            cfg.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            cfg.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
        });
    }
}