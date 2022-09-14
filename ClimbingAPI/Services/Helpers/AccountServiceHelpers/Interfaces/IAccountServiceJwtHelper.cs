using ClimbingAPI.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces
{
    public interface IAccountServiceJwtHelper
    {
        JwtSecurityToken GenerateToken(User user);
        IEnumerable<Claim> GenerateClaims(User user);
    }
}
