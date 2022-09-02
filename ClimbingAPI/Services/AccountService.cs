using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IUserContextService _userContext;
        private readonly IClimbingSpotService _climbingSpotService;

        public AccountService()
        {
            
        }
        public AccountService(IMapper mapper, ILogger<AccountService> logger, ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings settings, IAuthorizationService authorizationService, IUserContextService userContext, IClimbingSpotService climbingSpotService)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _settings = settings;
            _authorizationService = authorizationService;
            _userContext = userContext;
            _climbingSpotService = climbingSpotService;
        }
        public void Register(CreateUserDto dto)
        {
            _logger.LogInformation("INFO for: REGISTER action from AccountService.");

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            WhoColumns.CreationFiller(user, user.Id, DateTime.Now);

            var userClimbingSpot = CreateUserClimbingSpotEntity(user.Id, dto.RoleId);

            WhoColumns.CreationFiller(userClimbingSpot, user.Id, DateTime.Now);
            _dbContext.UserClimbingSpotLinks.Add(userClimbingSpot);
            _dbContext.SaveChanges();
        }

        private UserClimbingSpotLinks CreateUserClimbingSpotEntity(int userId, int roleId)
        {
            return new UserClimbingSpotLinks()
            {
                UserId = userId,
                ClimbingSpotId = null,
                RoleId = roleId
            };
        }

        public async Task<string> GenerateJwt(LoginUserDto dto)
        {
            _logger.LogInformation("INFO for: GENERATEJWT action from AccountService.");

            var user = await GetUserByEmail(dto.Email);
            if (user is null)
            {
                _logger.LogError("Invalid username or password.");
                throw new BadRequestException(Literals.Literals.InvalidUsernameOrPassowrd.GetDescription());
            }

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
                throw new BadRequestException(Literals.Literals.InvalidUsernameOrPassowrd.GetDescription());
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext
                .User
                .Where(x => x.Email == email)
                .Select(x => new User() { Id = x.Id, PasswordHash = x.PasswordHash, FirstName = x.FirstName, LastName = x.LastName, DateOfBirth = x.DateOfBirth })
                .FirstOrDefaultAsync();
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

        public async Task Update(UpdateUserDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: UPDATE action from AccountService. User Id: {userId}.");

            var authorizationResult = Authorize(ResourceOperation.Update, new AccountAuthorization() { UserId =  userId});
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: UPDATE action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var user = await GetUserById(userId);
            if(user is null)
            {
                _logger.LogError($"ERROR for: UPDATE action from ClimbingSpotService. User with id not found {userId}.");
                throw new NotFoundException($"User with id not found {userId}.");
            }

            user = UpdateUser(user, dto);
            WhoColumns.ModificationFiller(user, userId, DateTime.Now);

            _dbContext.Update(user);
            _dbContext.SaveChanges();
        }

        private User UpdateUser(User user, UpdateUserDto dto)
        {
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DateOfBirth = dto.DateOfBirth;

            return user;
        }

        private async Task<User> GetUserById(int userId)
        {
            return await _dbContext
                 .User
                 .Where(x => x.Id == userId)
                 .Select(x => new User() { Id = x.Id, PasswordHash = x.PasswordHash, Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, DateOfBirth = x.DateOfBirth, CreatedByUserId = x.CreatedByUserId, CreationDateTime = x.CreationDateTime})
                 .FirstOrDefaultAsync();
        }

        private AuthorizationResult Authorize(ResourceOperation resourceOperation, AccountAuthorization accountAuthorization)
        {
            return _authorizationService.AuthorizeAsync(_userContext.User, accountAuthorization,
                new ResourceOperationRequirement(resourceOperation)).Result;
        }

        public async Task ChangePassword(UpdateUserPasswordDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.ChangePasswordAction.GetDescription()} action from AccountService. User Id: {userId}.");

            var user = await VerifyUserData(userId, dto.OldPassword, Literals.Literals.ChangePasswordAction.GetDescription(), dto.NewPassword);

            UpdateUserPassword(dto, user);
        }

        private PasswordVerificationResult IsTheSamePassword(User user, string oldPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
        }

        private void UpdateUserPassword(UpdateUserPasswordDto dto, User user)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            WhoColumns.ModificationFiller(user, user.Id, DateTime.Now);

            _dbContext.User.Update(user);
            _dbContext.SaveChanges();
        }

        public async Task DeleteUser(DeleteUserDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.DeleteUserAction.GetDescription()} action from AccountService. User Id: {userId}.");

            var user = await VerifyUserData(userId, dto.Password, Literals.Literals.DeleteUserAction.GetDescription());

            await VerifyIfAnyClimbingSpotIsAssignedToUsec(userId);

            await RemoveUserAndUserClimbingSpotEntites(user, userId);
        }

        private async Task RemoveUserAndUserClimbingSpotEntites(User user, int userId)
        {
            var userClimbingSpotEntities = await GetUserClimbingSpotsEntities(userId);
            _dbContext.UserClimbingSpotLinks.RemoveRange(userClimbingSpotEntities);

            _dbContext.User.Remove(user);
            _dbContext.SaveChanges();
        }

        private async Task<List<UserClimbingSpotLinks>> GetUserClimbingSpotsEntities(int userId)
        {
            return await _dbContext
                .UserClimbingSpotLinks
                .Where(x => x.UserId == userId)
                .Select(x => new UserClimbingSpotLinks() { Id = x.Id})
                .ToListAsync();
        }

        private async Task VerifyIfAnyClimbingSpotIsAssignedToUsec(int userId)
        {
            var climbingSpots =  await _climbingSpotService.GetClimbingSpotAssignedToUser(userId);
            if (climbingSpots.Any())
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UserAssignedToClimbingSpot.GetDescription()} action from AccountService. User is assigned to Climbing Spot.");
                throw new BadRequestException(Literals.Literals.UserAssignedToClimbingSpot.GetDescription());
            }
        }

        private async Task<User> VerifyUserData(int userId, string password, string operation, string newPassword = null)
        {
            var authorizationResult = Authorize(ResourceOperation.Delete, new AccountAuthorization() { UserId = userId });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var user = await GetUserById(userId);
            if (user is null)
            {
                _logger.LogError($"ERROR for: {operation} action from AccountService. User not found.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var isPasswordCorrect = IsTheSamePassword(user, password);
            if (isPasswordCorrect == PasswordVerificationResult.Failed)
            {
                _logger.LogError($"ERROR for: {operation} action from AccountService. Wrong password.");
                throw new BadRequestException(Literals.Literals.InvalidPassowrd.GetDescription());
            }

            if(operation == Literals.Literals.ChangePasswordAction.GetDescription())
            {
                var isNewPasswordDifferentThanOld = IsTheSamePassword(user, newPassword);
                if (isNewPasswordDifferentThanOld == PasswordVerificationResult.Success)
                {
                    _logger.LogError($"ERROR for: {Literals.Literals.ChangePasswordAction.GetDescription()} action from AccountService. New passowrd is the same as old one. Please change it.");
                    throw new BadRequestException(Literals.Literals.PasswordsAreIdentical.GetDescription());
                }
            }

            return user;
        }
    }
}
