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

    public ProductController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
    public async Task<ActionResult<ResponseDto>> CreateProduct([FromBody] ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        try
        {
            var product = _mapper.Map<Product>(productDto);
            await _dbContext.Products.AddAsync(product);
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
    public async Task<ActionResult<ResponseDto>> UpdateProduct([FromBody] ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        try
        {
            var product = _mapper.Map<Product>(productDto);
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