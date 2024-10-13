using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CartController(ICartService cartService, IOrderService orderService)
    {
        ArgumentNullException.ThrowIfNull(cartService);
        ArgumentNullException.ThrowIfNull(orderService);

        _cartService = cartService;
        _orderService = orderService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    [HttpPost]
    [ActionName("Checkout")]
    public async Task<IActionResult> Checkout(CartDto cartDto)
    {
        CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
        cart.CartHeader.Phone = cartDto.CartHeader.Phone;
        cart.CartHeader.Email = cartDto.CartHeader.Email;
        cart.CartHeader.Name = cartDto.CartHeader.Name;

        var response = await _orderService.CreateOrderAsync(cart);

        if (response == null || !response.IsSuccess)
        {
            TempData["error"] = "Order creation failed";
            return View(cart);
        }

        var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

        return View();
    }

    public async Task<IActionResult> Confirmation(int orderId)
    {
        return View(orderId);
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string userId, string couponCode)
    {
        var response = await _cartService.ApplyCouponAsync(userId, couponCode);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(string userId)
    {
        var response = await _cartService.RemoveCouponAsync(userId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart(CartDto cartDto)
    {
        if (cartDto.CartDetails == null || !cartDto.CartDetails.Any())
        {
            TempData["error"] = "Cart is empty";
            return RedirectToAction(nameof(CartIndex));
        }

        var cartDetails = cartDto.CartDetails.ToList();
        foreach (var dto in cartDetails)
        {
            if (dto.Product?.ImageURL == null)
            {
                dto.Product.ImageURL = string.Empty;
            }
        }

        var email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
        cartDto.CartHeader.Email = email;

        var response = await _cartService.EmailCartAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Email will be processed and sent shortly";
            return RedirectToAction(nameof(CartIndex));
        }

        TempData["error"] = "Email processing failed";
        return RedirectToAction(nameof(CartIndex));
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        var response = await _cartService.GetCartByUserIdAsync(userId);
        if (response != null && response.IsSuccess)
        {
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            return cartDto;
        }

        return new CartDto();
    }
}