using Mango.Services.AuthAPI.Model.Dtos;
using Mango.Services.AuthAPI.Service.IService;
using MessageBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthAPIController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMessageBus _messageBus;
    private readonly AwsOptions _awsOptions;
    private readonly ResponseDto _response;

    public AuthAPIController(IAuthService authService, IMessageBus messageBus, IOptions<AwsOptions> awsOptions)
    {
        ArgumentNullException.ThrowIfNull(authService);
        ArgumentNullException.ThrowIfNull(messageBus);
        ArgumentNullException.ThrowIfNull(awsOptions);

        _authService = authService;
        _messageBus = messageBus;
        _awsOptions = awsOptions.Value;
        _response = new();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequestDto requestDto)
    {
        var (errorMessage, user) = await _authService.Register(requestDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            _response.IsSuccess = false;
            _response.Message = errorMessage;
            return BadRequest(_response);
        }

        if (user == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Unable to register user";
            return BadRequest("Unable to register user");
        }

        await _messageBus.PublishMessage(user, _awsOptions.UserRegistrationQueue);
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