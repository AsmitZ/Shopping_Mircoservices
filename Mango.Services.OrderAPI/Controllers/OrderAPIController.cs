using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers;

[Route("api/orders")]
public class OrderAPIController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public OrderAPIController(AppDbContext dbContext, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(mapper);

        _dbContext = dbContext;
        _mapper = mapper;
        _response = new ResponseDto();
    }

    [Authorize]
    [HttpPost]
    public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
    {
        try
        {
            var orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
            orderHeaderDto.UserId = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            orderHeaderDto.Status = SD.Status.Pending;
            orderHeaderDto.OrderTime = DateTime.Now;
            orderHeaderDto.OrderDetails = _mapper.Map<List<OrderDetailsDto>>(cartDto.CartDetails);

            var createdOrder = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
            await _dbContext.SaveChangesAsync();

            orderHeaderDto.OrderHeaderId = createdOrder.OrderHeaderId;
            _response.Result = orderHeaderDto;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }

    [Authorize]
    [HttpPost("session")]
    public async Task<ResponseDto> CreatePaymentSession([FromBody] PaymentRequestDto paymentRequestDto)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = paymentRequestDto.OnSuccessUrl,
            CancelUrl = paymentRequestDto.OnCancelUrl,
            LineItems = [],
            Discounts = []
        };

        if (paymentRequestDto.OrderHeader.Discount > 0)
        {
            var discount = new SessionDiscountOptions
            {
                Coupon = paymentRequestDto.OrderHeader.CouponCode,
            };
            options.Discounts.Add(discount);
        }

        foreach (var orderItem in paymentRequestDto.OrderHeader.OrderDetails)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)orderItem.ProductPrice * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderItem.Product.Name,
                    },
                },
                Quantity = orderItem.Count,
            };

            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = await service.CreateAsync(options);
        paymentRequestDto.SessionUrl = session.Url;
        paymentRequestDto.SessionId = session.Id;
        OrderHeader orderHeader =
            _dbContext.OrderHeaders.First(o => o.OrderHeaderId == paymentRequestDto.OrderHeader.OrderHeaderId);
        orderHeader.StripeSessionId = session.Id;
        await _dbContext.SaveChangesAsync();
        _response.Result = paymentRequestDto;
        return _response;
    }


    [Authorize]
    [HttpPost("session/validate")]
    public async Task<ResponseDto> ValidatePaymentSession([FromBody] int orderHeaderId)
    {
        try
        {
            var orderHeader = _dbContext.OrderHeaders.First(o => o.OrderHeaderId == orderHeaderId);

            var service = new SessionService();
            Session session = await service.GetAsync(orderHeader.StripeSessionId);

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);
            if (paymentIntent.Status != "succeeded")
            {
                return _response;
            }

            orderHeader.PaymentIntentId = paymentIntent.Id;
            orderHeader.Status = SD.Status.Approved;
            await _dbContext.SaveChangesAsync();

            _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }
}