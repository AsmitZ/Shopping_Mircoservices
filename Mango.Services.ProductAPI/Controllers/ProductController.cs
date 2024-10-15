using AutoMapper;
using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers;

[ApiController]
[Route("api/products")]
// [Authorize]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private IWebHostEnvironment _hostEnvironment;

    public ProductController(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _hostEnvironment = hostEnvironment;
        _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetProducts()
    {
        try
        {
            var products = await _dbContext.Products.ToListAsync();
            _response.Result = _mapper.Map<List<ProductDto>>(products);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return Ok(_response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ResponseDto>> GetProductById(long id)
    {
        try
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Product not found";
            }
            else
            {
                _response.Result = _mapper.Map<ProductDto>(product);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return Ok(_response);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> CreateProduct(ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        try
        {
            var product = _mapper.Map<Product>(productDto);
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            if (productDto.Image != null)
            {
                var fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);

                // Get the physical path to wwwroot
                var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file to the physical path
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await productDto.Image.CopyToAsync(fileStream);

                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                product.ImageURL = baseUrl + "/ProductImages/" + fileName;
                product.ImageLocalPath = filePath;
            }
            else
            {
                product.ImageURL = "https://placehold.co/600x400";
            }

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            _response.Result = _mapper.Map<ProductDto>(product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return Ok(_response);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> UpdateProduct(ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        try
        {
            var product = _mapper.Map<Product>(productDto);

            if (productDto.Image != null)
            {
                var fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);

                // Get the physical path to wwwroot
                var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file to the physical path
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await productDto.Image.CopyToAsync(fileStream);

                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                product.ImageURL = baseUrl + "/ProductImages/" + fileName;
                product.ImageLocalPath = filePath;
            }
            else
            {
                var productFromDb = await _dbContext.Products.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                product.ImageURL = productFromDb.ImageURL;
                product.ImageLocalPath = productFromDb.ImageLocalPath;
            }

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            _response.Result = _mapper.Map<ProductDto>(product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return Ok(_response);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> DeleteProduct(long id)
    {
        try
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Product not found";
            }
            else
            {
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    if (System.IO.File.Exists(product.ImageLocalPath))
                    {
                        System.IO.File.Delete(product.ImageLocalPath);
                    }
                }

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                _response.Result = _mapper.Map<ProductDto>(product);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return Ok(_response);
    }
}