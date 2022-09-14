using ClimbingAPI.Entities;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClimbingAPI.Services.Helpers
{
    public class AccountServiceJwtHelper: IAccountServiceJwtHelper
    {
        private readonly AuthenticationSettings _settings;
        public AccountServiceJwtHelper(AuthenticationSettings settings)
        {
            _settings = settings;
        }
        public AccountServiceJwtHelper()
        {

        }
        public JwtSecurityToken GenerateToken(User user)
        {
            var claims = GenerateClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(_settings.JwtExpireDays);

            var token = new JwtSecurityToken(_settings.JwtIssuer, _settings.JwtIssuer, claims, expires: expires,
                signingCredentials: credentials);

            return token;
        }
        public IEnumerable<Claim> GenerateClaims(User user)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };
        }
    }
}
