using System.Text;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Services;

public class EmailService : IEmailService
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public EmailService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public async Task<bool> SendAndLogEmail(CartDto cartDto)
    {
        var emailContent = GenerateEmailContent(cartDto);
        await using var dbContext = new AppDbContext(_dbOptions);
        dbContext.EmailLoggers.Add(emailContent);
        return await dbContext.SaveChangesAsync() > 0;
    }

    private EmailLogger GenerateEmailContent(CartDto cartDto)
    {
        var sb = new StringBuilder();
        sb.Append("<html><body>");
        sb.Append("<h1>Your Order Details</h1>");
        sb.Append("<hr/>");
        foreach (var cartDetail in cartDto.CartDetails)
        {
            sb.Append($"<div>Product: {cartDetail.Product.Name}</div>");
            sb.Append($"<div>Quantity: {cartDetail.Count}</div>");
            sb.Append($"<div>Price: {cartDetail.Product.Price}</div>");
            sb.Append("<hr/>");
        }

        sb.Append("<h4>Order Number: " + cartDto.CartHeader.CartHeaderId + "</h4>");
        sb.Append("<h4>Order Total: " + cartDto.CartHeader.CartTotal + "</h4>");
        sb.Append("</body></html>");
        var message = sb.ToString();

        return new EmailLogger
        {
            Id = Guid.NewGuid().ToString(),
            Email = cartDto.CartHeader.Email,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }
}