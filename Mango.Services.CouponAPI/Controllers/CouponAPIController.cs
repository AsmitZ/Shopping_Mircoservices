using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Coupon = Mango.Services.CouponAPI.Models.Coupon;

namespace Mango.Services.CouponAPI.Controllers;

[Route("api/coupons")]
[ApiController]
[Authorize]
public class CouponApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private ResponseDto _responseDto;

    public CouponApiController(AppDbContext dbContext, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(mapper);
        _dbContext = dbContext;
        _mapper = mapper;
        _responseDto = new ResponseDto();
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateCoupon([FromBody] CouponDto couponDto)
    {
        ArgumentNullException.ThrowIfNull(couponDto);
        try
        {
            var coupon = _mapper.Map<Coupon>(couponDto);
            await _dbContext.Coupons.AddAsync(coupon);
            await _dbContext.SaveChangesAsync();
            _responseDto.Result = _mapper.Map<CouponDto>(coupon);

            var options = new CouponCreateOptions
            {
                Id = coupon.CouponCode,
                AmountOff = (long)(coupon.DiscountAmount * 100),
                Currency = "usd",
                Name = coupon.CouponCode
            };
            var service = new CouponService();
            await service.CreateAsync(options);
        }
        catch (Exception e)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = e.Message;
        }

        return Ok(_responseDto);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetCoupons()
    {
        try
        {
            var coupons = await _dbContext.Coupons.ToListAsync();
            _responseDto.Result = _mapper.Map<List<CouponDto>>(coupons);
        }
        catch (Exception e)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = e.Message;
        }

        return Ok(_responseDto);
    }

    [HttpGet("{couponId:int}")]
    public async Task<ActionResult<ResponseDto>> GetCouponById(int couponId)
    {
        try
        {
            var coupon = await _dbContext.Coupons.FirstAsync(c => c.CouponId == couponId);
            _responseDto.Result = _mapper.Map<CouponDto>(coupon);
        }
        catch (Exception e)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = e.Message;
        }

        return Ok(_responseDto);
    }

    [HttpGet("{couponCode}")]
    public async Task<ActionResult<ResponseDto>> GetCouponByCode(string couponCode)
    {
        try
        {
            var coupon =
                await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());
            if (coupon == null)
            {
                return NotFound();
            }

            _responseDto.Result = _mapper.Map<CouponDto>(coupon);
        }
        catch (Exception e)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = e.Message;
        }

        return Ok(_responseDto);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> UpdateCoupon([FromBody] CouponDto couponDto)
    {
        try
        {
            var coupon = _mapper.Map<Coupon>(couponDto);
            _dbContext.Coupons.Update(coupon);
            await _dbContext.SaveChangesAsync();
            _responseDto.Result = _mapper.Map<CouponDto>(coupon);
        }
        catch (Exception e)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = e.Message;
        }

        return Ok(_responseDto);
    }

    [HttpDelete("{couponId:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteCoupon(int couponId)
    {
        if (couponId == default)
        {
            throw new ArgumentException("Coupon Id cannot be 0", nameof(couponId));
        }

        try
        {
            var coupon = await _dbContext.Coupons.FirstAsync(c => c.CouponId == couponId);
            _dbContext.Coupons.Remove(coupon);
            await _dbContext.SaveChangesAsync();

            var service = new CouponService();
            await service.DeleteAsync(coupon.CouponCode);

            return Ok(coupon);
        }
        catch (Exception e)
        {
            return BadRequest($"Unable to delete the coupon, due to {e.Message}");
        }
    }
}