using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IAuthService
{
    Task<ResponseDto?> LoginAsync(LoginRequestDto model);
    Task<ResponseDto?> RegisterAsync(RegistrationRequestDto model);
    Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto model);
}