using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;

    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto loginRequestDto = new();
        return View(loginRequestDto);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result is not null && result.IsSuccess)
        {
            LoginResponseDto response = JsonConvert.DeserializeObject<LoginResponseDto>(result.Result.ToString());

            await SignInUser(response);
            _tokenProvider.SetToken(response.Token);
            return RedirectToAction(nameof(Index), "Home");
        }

        TempData["error"] = result.Message;
        return View(dto);
    }

    [HttpGet]
    public IActionResult Register()
    {
        var RoleList = new List<SelectListItem>
        {
            new() { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
            new() { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
        };

        ViewBag.RoleList = RoleList;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result is not null || result.IsSuccess)
        {
            if (string.IsNullOrEmpty(dto.Role))
            {
                dto.Role = SD.RoleCustomer;
            }

            var assignResponse = await _authService.AssignRoleAsync(dto);

            if (assignResponse is not null && assignResponse.IsSuccess)
            {
                TempData["Success"] = "Registration successful.";
                return RedirectToAction(nameof(Login));
            }
        }
        else
        {
            TempData["error"] = result.Message;
        }

        var RoleList = new List<SelectListItem>
        {
            new() { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
            new() { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
        };

        ViewBag.RoleList = RoleList;
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        return RedirectToAction(nameof(Login));
    }

    private async Task SignInUser(LoginResponseDto dto)
    {
        var handler = new JwtSecurityTokenHandler();
        var token =  handler.ReadJwtToken(dto.Token);
        var claims = new List<Claim>();
        claims.Add(new Claim(JwtRegisteredClaimNames.Email,
            token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
        claims.Add(new Claim(JwtRegisteredClaimNames.Name,
            token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub,
            token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
        
        claims.Add(new Claim(ClaimTypes.Name,
            token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        claims.Add(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(u => u.Type == "role").Value));
        
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaims(claims);

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);
    }
}