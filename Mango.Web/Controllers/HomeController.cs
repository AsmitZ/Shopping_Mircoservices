using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    public HomeController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto>? products = new();
        var response = await _productService.GetAllProductsAsync();
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Received products from ProductService.");
            products = JsonConvert.DeserializeObject<List<ProductDto>>(
                Convert.ToString(response.Result) ?? string.Empty);
        }
        else
        {
            TempData["Error"] = response?.Message;
        }

        Console.WriteLine("Rendering ProductIndex view...");
        return View(products);
    }
    
    [Authorize]
    public async Task<IActionResult> ProductDetails(long productId)
    {
        ProductDto? product = new();
        var response = await _productService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Received products from ProductService.");
            product = JsonConvert.DeserializeObject<ProductDto>(
                Convert.ToString(response.Result) ?? string.Empty);
        }
        else
        {
            TempData["Error"] = response?.Message;
        }

        Console.WriteLine("Rendering ProductIndex view...");
        return View(product);
    }
    
    [Authorize]
    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductDto productDto)
    {
        var cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
            }
        };
        var cartDetail = new CartDetailsDto
        {
            Count = productDto.Count,
            ProductId = (int)productDto.ProductId
        };
        cartDto.CartDetails = new[] { cartDetail };
        var response = await _cartService.UpsertCartAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["Success"] = "Item added to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = response?.Message;

        Console.WriteLine("Rendering ProductIndex view...");
        return View(productDto);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}