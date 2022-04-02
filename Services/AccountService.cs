using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ClimbingAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly ClimbingDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _settings;

        public AccountService(IMapper mapper, ILogger<AccountService> logger, ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings settings)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _settings = settings;
        }
        public void Register(CreateUserDto dto)
        {
            _logger.LogInformation("INFO for: REGISTER action from AccountService.");

            var user = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = dto.RoleId
            };

            var hashedPassword = _passwordHasher.HashPassword(user, dto.Password);
            user.PasswordHash = hashedPassword;

            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
        }

        public string GenerateJwt(LoginUserDto dto)
        {
            _logger.LogInformation("INFO for: GENERATEJWT action from AccountService.");

            var user = GetUser(dto.Email);

            VerifyHashedPassword(dto, user);

            var token = GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken GenerateToken(User user)
        {
            var claims = GenerateClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(_settings.JwtExpireDays);

            var token = new JwtSecurityToken(_settings.JwtIssuer, _settings.JwtIssuer, claims, expires: expires,
                signingCredentials: credentials);

            return token;
        }

        private void VerifyHashedPassword(LoginUserDto dto, User user)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogError("Invalid username or password.");
                throw new BadRequestException("Invalid username or password.");
            }
        }

        private User GetUser(string email)
        {
            var user = _dbContext.User
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == email);
            if (user is null)
            {
                _logger.LogError("Invalid username or password.");
                throw new BadRequestException("Invalid username or password.");
            }

            return user;
        }

        private IEnumerable<Claim> GenerateClaims(User user)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };
        }
    }
}
