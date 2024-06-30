using Mango.Services.AuthAPI.Model.Dtos;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthAPIController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ResponseDto _response;

    public AuthAPIController(IAuthService authService)
    {
        _authService = authService;
        _response = new();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequestDto requestDto)
    {
        var errorMessage = await _authService.Register(requestDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            _response.IsSuccess = false;
            _response.Message = errorMessage;
            return BadRequest(_response);
        }

        return Ok(_response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto requestDto)
    {
        var loginResponse = await _authService.Login(requestDto);
        if (loginResponse.User == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Login failed";
            return BadRequest(_response);
        }

        _response.Result = loginResponse;
        return Ok(_response);
    }
    
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(RegistrationRequestDto requestDto)
    {
        var isSuccess = await _authService.AssignRole(requestDto.Email, requestDto.Role);
        if (!isSuccess)
        {
            _response.IsSuccess = false;
            _response.Message = "Failed to assign role";
            return BadRequest(_response);
        }
        
        return Ok(_response);
    }
}