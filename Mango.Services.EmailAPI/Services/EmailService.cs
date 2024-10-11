using System.Text;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Services;

public class EmailService(DbContextOptions<AppDbContext> dbOptions) : IEmailService
{
    public async Task<bool> SendAndLogEmail<T>(T emailBody)
    {
        var emailContent = GenerateEmail(emailBody);

        return await SendAndLogEmail(emailContent);
    }

    private async Task<bool> SendAndLogEmail(EmailLogger emailContent)
    {
        await using var dbContext = new AppDbContext(dbOptions);
        dbContext.EmailLoggers.Add(emailContent);
        return await dbContext.SaveChangesAsync() > 0;
    }

    private EmailLogger GenerateEmail<T>(T dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto is CartDto cartDto)
        {
            return BuildCartEmail(cartDto);
        }

        //TODO: Add user registered email template

        throw new InvalidOperationException($"Unsupported email type {dto.GetType()}");
    }

    private static EmailLogger BuildCartEmail(CartDto cartDto)
    {
        ArgumentNullException.ThrowIfNull(cartDto);

        if (string.IsNullOrWhiteSpace(cartDto.CartHeader.Email))
        {
            throw new InvalidOperationException("Cart email should not be null");
        }

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