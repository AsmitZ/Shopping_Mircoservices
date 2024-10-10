using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI.Services;

public interface IEmailService
{
    Task<bool> SendAndLogEmail(CartDto cartDto);
}