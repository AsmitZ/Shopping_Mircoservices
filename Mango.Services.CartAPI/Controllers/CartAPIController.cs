using AutoMapper;
using Mango.Services.CartAPI.Data;
using Mango.Services.CartAPI.Models;
using Mango.Services.CartAPI.Models.Dto;
using Mango.Services.CartAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class CartAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private ResponseDto _response;

    public CartAPIController(IMapper mapper, AppDbContext db, IProductService productService, ICouponService couponService)
    {
        _mapper = mapper;
        _db = db;
        _productService = productService;
        _couponService = couponService;
        _response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<ActionResult<ResponseDto>> GetCart(string userId)
    {
        try
        {
            var cartHeader = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cartHeader is null)
            {
                _response.IsSuccess = false;
                _response.Message = "No cart found";
                return _response;
            }

            var cartHeaderDto = _mapper.Map<CartHeaderDto>(cartHeader);
            var cartDetails = await _db.CartDetails.Where(x => x.CartHeaderId == cartHeader.CartHeaderId).ToListAsync();
            var cartDetailsDto = _mapper.Map<List<CartDetailsDto>>(cartDetails);
            var products = await _productService.GetProducts();

            cartDetailsDto.ForEach(cartDetail =>
                cartDetail.Product = products.FirstOrDefault(x => x.ProductId == cartDetail.ProductId));
            
            cartHeaderDto.CartTotal = (double)cartDetailsDto.Sum(cart => cart.Count * cart.Product.Price);

            if (!string.IsNullOrWhiteSpace(cartHeaderDto.CouponCode))
            {
                var couponDto = await _couponService.GetCoupon(cartHeaderDto.CouponCode);
                if (couponDto is not null && cartHeaderDto.CartTotal > couponDto.MinAmount)
                {
                    cartHeaderDto.CartTotal -= couponDto.DiscountAmount;
                    cartHeaderDto.Discount = couponDto.DiscountAmount;
                }
            }

            var cartDto = new CartDto { CartHeader = cartHeaderDto, CartDetails = cartDetailsDto };
            
            _response.Result = cartDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message ;
        }

        return _response;
    } 

    [HttpPost("CartUpsert")]
    public async Task<ActionResult<ResponseDto>> Upsert([FromBody] CartDto cartDto)
    {
        try
        {
            var cartHeader = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
            if (cartHeader == null)
            {
                var cartHeaderObj = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _db.CartHeaders.Add(cartHeaderObj);
                await _db.SaveChangesAsync();
                cartDto.CartHeader.CartHeaderId = cartHeaderObj.CartHeaderId;
                cartDto.CartDetails.First().CartHeaderId = cartHeaderObj.CartHeaderId;
                var cartDetailObj = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                _db.CartDetails.Add(cartDetailObj);
                await _db.SaveChangesAsync();
            }
            else
            {
                // if header is not null
                // check if details has the same product
                var cartDetails = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(detail =>
                    detail.ProductId == cartDto.CartDetails.First().ProductId && 
                    detail.CartHeaderId == cartHeader.CartHeaderId);

                if (cartDetails == null)
                {
                    var cartDetailObj = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                    cartDetailObj.CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(cartDetailObj);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cartDto.CartDetails.First().Count += cartDetails.Count;
                    cartDto.CartDetails.First().CartDetailsId = cartDetails.CartDetailsId;
                    cartDto.CartDetails.First().CartHeaderId = cartDetails.CartHeaderId;
                    var cartDetailObj = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                    _db.CartDetails.Update(cartDetailObj);
                    await _db.SaveChangesAsync();
                }
            }

            _response.Result = cartDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message ;
        }
        return _response;
    }
    
    [HttpPost("RemoveCart")]
    public async Task<ActionResult<ResponseDto>> RemoveCart(int cartDetailId)
    {
        try
        {
            var cartDetails = await _db.CartDetails.FirstAsync(cart => cart.CartDetailsId == cartDetailId);
            var totalCartDetailCount =
                _db.CartDetails.Count(cart => cart.CartHeader.CartHeaderId == cartDetails.CartHeaderId);
            
            _db.Remove(cartDetails);
            
            if (totalCartDetailCount == 1)
            {
                var cartHeader = await _db.CartHeaders.FirstAsync(cart => cart.CartHeaderId == cartDetails.CartHeaderId);
                _db.Remove(cartHeader);
            }

            await _db.SaveChangesAsync();

            _response.Result = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message ;
        }
        return _response;
    }

    [HttpPut("ApplyCoupon/{userId}")]
    public async Task<ActionResult<ResponseDto>> ApplyCoupon([FromRoute]string userId, string couponCode)
    {
        var cartFromDb = await _db.CartHeaders.FirstAsync(cart => cart.UserId == userId);
        if (cartFromDb is null)
        {
            return BadRequest("Unable to find shopping cart");
        }

        cartFromDb.CouponCode = couponCode;
        _db.CartHeaders.Update(cartFromDb);
        await _db.SaveChangesAsync();
        
        _response.Message = "Coupon applied successfully";
        _response.Result = cartFromDb;
        return _response;
    }
    
    [HttpPut("RemoveCoupon/{userId}")]
    public async Task<ActionResult<ResponseDto>> RemoveCoupon([FromRoute] string userId)
    {
        var cartFromDb = await _db.CartHeaders.FirstAsync(cart => cart.UserId == userId);
        if (cartFromDb is null)
        {
            return BadRequest("Unable to find shopping cart");
        }

        cartFromDb.CouponCode = "";
        _db.CartHeaders.Update(cartFromDb);
        await _db.SaveChangesAsync();
        
        _response.Message = "Coupon removed successfully";
        return _response;
    }
}

// {
//     "cartHeader": {
//         "cartHeaderId": 0,
//         "userId": "string",
//         "couponCode": "string",
//         "cartTotal": 0,
//         "discount": 0
//     },
//     "cartDetails": [
//     {
//         "cartDetailsId": 0,
//         "cartHeaderId": 0,
//         "productId": 0,
//         "count": 0
//     }
//     ]
// }

    