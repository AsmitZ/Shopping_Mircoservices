using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Model;
using Mango.Services.AuthAPI.Model.Dtos;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Register(RegistrationRequestDto model)
    {
        ApplicationUser user = new ApplicationUser()
        {
            UserName = model.Email,
            Email = model.Email,
            NormalizedEmail = model.Email.ToUpper(),
            Name = model.Name,
            PhoneNumber = model.PhoneNumber
        };

        try
        {
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userToReturn = _db.ApplicationUsers.First(u => u.Email == model.Email);

                var registerUser = new UserDto()
                {
                    Id = userToReturn.Id,
                    Name = userToReturn.Name,
                    Email = userToReturn.Email,
                    PhoneNumber = userToReturn.PhoneNumber
                };
                return "";
            }

            return result.Errors.First().Description;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Error occurred while registering the user";
        }
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto model)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(user => user.Email == model.UserName);

        var isValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (user == null || !isValid)
        {
            return new LoginResponseDto() { User = null, Token = "" };
        }

        var userDto = new UserDto()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        var loginResponse = new LoginResponseDto
        {
            User = userDto,
            Token = token
        };

        return loginResponse;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(user => user.Email == email);
        
        if (user == null)
        {
            return false;
        }

        var roleExists = _roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
        if (!roleExists)
        {
            _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        }
        await _userManager.AddToRoleAsync(user, roleName);
        return true;
    }
}