using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        Console.WriteLine("Requesting products from ProductService...");
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
    
    public async Task<IActionResult> ProductCreate()
    {
        Console.WriteLine("Rendering ProductCreate view...");
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid product data.";
            return View(productDto);
        }

        try
        {
            var response = await _productService.CreateProductAsync(productDto);
            if (response != null && response.IsSuccess)
            {
                Console.WriteLine("Product created successfully.");
                return RedirectToAction(nameof(ProductIndex));
            }
            TempData["Error"] = response?.Message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TempData["Error"] = e.Message;
        }

        return View(productDto);
    }
    
    public async Task<IActionResult> ProductEdit(long productId)
    {
        try
        {
            var response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                Console.WriteLine("Rendering ProductEdit view...");
                return View(JsonConvert.DeserializeObject<ProductDto>(
                    Convert.ToString(response.Result) ?? string.Empty));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TempData["Error"] = e.Message;
        }
        
        return RedirectToAction(nameof(ProductIndex));
    }
    
    [HttpPost]
    public async Task<IActionResult> ProductEdit(ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid product data.";
            return View(productDto);
        }

        var response = await _productService.UpdateProductAsync(productDto);
        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Product updated successfully.");
            return RedirectToAction(nameof(ProductIndex));
        }

        TempData["Error"] = response?.Message;
        return View(productDto);
    }
    
    public async Task<IActionResult> ProductDelete(long productId)
    {
        try
        {
            var response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(
                    Convert.ToString(response.Result) ?? string.Empty);
                return View(productDto);
            }
            TempData["Error"] = response?.Message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TempData["Error"] = e.Message;
        }
        
        return RedirectToAction(nameof(ProductIndex));
    }
    
    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDto productDto)
    {
        try
        {
            var response = await _productService.DeleteProductAsync(productDto.ProductId);
            if (response != null && response.IsSuccess)
            {
                Console.WriteLine("Product deleted successfully.");
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(ProductIndex));
            }
            TempData["Error"] = response?.Message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TempData["Error"] = e.Message;
        }
        
        return RedirectToAction(nameof(ProductIndex));
    }
}