using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthorizationService _authorizationService;

        public AccountService(IMapper mapper, ILogger<AccountService> logger, ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings settings, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _settings = settings;
            _authorizationService = authorizationService;
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

            var userClimbingSpot = CreateUserClimbingSpotEntity(user.Id, dto.RoleId);
            
            _dbContext.UserClimbingSpot.Add(userClimbingSpot);
            _dbContext.SaveChanges();
        }

        private UserClimbingSpot CreateUserClimbingSpotEntity(int userId, int roleId)
        {
            return new UserClimbingSpot()
            {
                UserId = userId,
                ClimbingSpotId = null,
                RoleId = roleId
            };
        }

        public string GenerateJwt(LoginUserDto dto)
        {
            _logger.LogInformation("INFO for: GENERATEJWT action from AccountService.");

            var user = GetUserByEmail(dto.Email);

            VerifyHashedPassword(dto, user);

            var token = GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public void UpdateRole(UpdateUserRoleDto dto, ClaimsPrincipal userClaims, int userId)
        {
            _logger.LogInformation("INFO for: UPDATEROLE action from AccountService.");

            var user = GetUserById(userId);

            var authorizationResult = _authorizationService.AuthorizeAsync(userClaims, user,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: UPDATEROLE action from ClimbingSpotService. Authorization failed.");
                throw new ForbidException($"Authorization failed.");
            }

            user.Role.Id = dto.RoleId;

            _dbContext.Update(user);
            _dbContext.SaveChanges();


        }

        public void AssignClimbingSpotToUser(UpdateUserClimbingSpotDto dto, ClaimsPrincipal user)
        {
            _logger.LogInformation("INFO for: AssignClimbingSpotToUser action from AccountService.");

            Validate(dto.UserId, dto.ClimbingSpotId, user);

            var userClimbingSpotEntity = ShouldUserBeCreated(dto.UserId);
            
            if (userClimbingSpotEntity is null)
            {
                var userClimbingSpot = new UserClimbingSpot()
                {
                    ClimbingSpotId = dto.ClimbingSpotId,
                    UserId = dto.UserId,
                    RoleId = dto.RoleId
                };
                _dbContext.UserClimbingSpot.Add(userClimbingSpot);
            }
            else
            {
                userClimbingSpotEntity.ClimbingSpotId = dto.ClimbingSpotId;
                userClimbingSpotEntity.UserId = dto.UserId;
                userClimbingSpotEntity.RoleId = dto.RoleId;
            }
            _dbContext.SaveChanges();
        }

        private UserClimbingSpot ShouldUserBeCreated(int userId)
        {
            return _dbContext.UserClimbingSpot.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == null);
        }

        private void Validate(int userId, int climbingSpotId, ClaimsPrincipal userPrincipal)
        {
            var user = _dbContext.User.FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException($"User with ID: {userId} not exists.");

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (climbingSpot is null)
                throw new NotFoundException($"Climbing spot with ID: {climbingSpotId} not exists.");

            //checking if user is assigned to climbing spot. If not means that is not a manager or admin in that climbingSpot
            var userClaimId = userPrincipal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userAssignedToClimbingSpot = _dbContext.UserClimbingSpot.FirstOrDefault(x =>
                x.UserId == int.Parse(userClaimId) && x.ClimbingSpotId == climbingSpotId);
            if(userAssignedToClimbingSpot is null)
                throw new BadRequestException(
                    $"User with ID: {userClaimId} is not assigned to climbing spot: {climbingSpotId}.");

            var userClimbingSpotEntity =
                _dbContext.UserClimbingSpot.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == climbingSpotId);
            if (userClimbingSpotEntity is not null)
                throw new BadRequestException(
                    $"User with ID: {userId} already assigned to climbing spot with ID: {climbingSpotId}.");

            
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

        private User GetUserByEmail(string email)
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

        private User GetUserById(int id)
        {
            var user = _dbContext.User
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == id);
            if (user is null)
            {
                _logger.LogError($"No content. User with id: {id} not found.");
                throw new NotFoundException($"No content. User with id: {id} not found.");
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
