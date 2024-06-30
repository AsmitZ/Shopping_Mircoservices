using System.Collections;
using Mango.Services.AuthAPI.Model;

namespace Mango.Services.AuthAPI.Service.IService;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser userDto, IEnumerable<string> roles);
}