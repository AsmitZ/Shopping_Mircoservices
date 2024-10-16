using Mango.Services.AuthAPI.Model.Dtos;

namespace Mango.Services.AuthAPI.Service.IService;

public interface IAuthService
{
    Task<(string? errorMessage, UserDto? user)> Register(RegistrationRequestDto model);
    Task<LoginResponseDto> Login(LoginRequestDto model);
    Task<bool> AssignRole(string email, string roleName);
}