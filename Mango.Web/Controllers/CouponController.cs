using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;
    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }
    
    public async Task<IActionResult> CouponIndex()
    {
        
        Console.WriteLine("Requesting coupons from CouponService...");
        List<CouponDto>? coupons = new();
        var response = await _couponService.GetAllCouponsAsync();
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Received coupons from CouponService.");
            coupons = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result) ?? string.Empty);
        }
        else
        {
            TempData["Error"] = response?.Message;
        }

        Console.WriteLine("Rendering CouponIndex view...");
        return View(coupons);
    }

    public IActionResult CouponCreate()
    {
        Console.WriteLine("Rendering CreateCoupon view...");
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDto model)
    {
        Console.WriteLine("Saving CreateCoupon model...");
        if (ModelState.IsValid)
        {
            Console.WriteLine("Model is valid. Saving model...");
            var response = await _couponService.CreateCouponAsync(model);
            
            if (response != null && response.IsSuccess)
            {
                Console.WriteLine("Model saved successfully.");
                TempData["Success"] = "Coupon created successfully.";
                return RedirectToAction("CouponIndex");
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
        }
        return View(model);
    }

    public async Task<IActionResult> CouponView(int couponId)
    {
        Console.WriteLine("Viewing coupon with Id " + couponId);
        var response = await _couponService.GetCouponByIdAsync(couponId);
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Rendering CouponView view...");
            var model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result) ?? string.Empty);
            return View(model);
        }
        else
        {
            TempData["Error"] = response?.Message;
        }

        return NotFound();
    }
    
    public async Task<IActionResult> CouponDelete(int couponId)
    {
        Console.WriteLine("Deleting coupon...");
        var response = await _couponService.DeleteCouponAsync(couponId);
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Coupon deleted successfully.");
            TempData["Success"] = "Coupon deleted successfully.";
            return RedirectToAction("CouponIndex");
        }
        else
        {
            TempData["Error"] = response?.Message;
        }
        return RedirectToAction("CouponIndex");
    }
}