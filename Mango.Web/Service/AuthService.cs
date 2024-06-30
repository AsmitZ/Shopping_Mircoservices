using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;
    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    public async Task<ResponseDto?> LoginAsync(LoginRequestDto model)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = model,
            Url = SD.AuthApiBase + "api/auth/login"
        }, false);
    }

    public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto model)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = model,
            Url = SD.AuthApiBase + "api/auth/register"
        }, false);
    }

    public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto model)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = model,
            Url = SD.AuthApiBase + "api/auth/assign-role"
        });
    }
}