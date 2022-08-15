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

        public string GenerateJwt(LoginUserDto dto)
        {
            _logger.LogInformation("INFO for: GENERATEJWT action from AccountService.");

            var user = GetUserByEmail(dto.Email);
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

        public User GetUserByEmail(string email)
        {
            return _dbContext.User
                .FirstOrDefault(x => x.Email == email);
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

        public void Update(UpdateUserDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: UPDATE action from AccountService. User Id: {userId}.");

            var authorizationResult = Authorize(ResourceOperation.Update, new AccountAuthorization() { UserId =  userId});
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: UPDATE action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var user = GetUserById(userId);
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

        private User GetUserById(int userId)
        {
            return _dbContext.User.FirstOrDefault(x => x.Id == userId);
        }

        private AuthorizationResult Authorize(ResourceOperation resourceOperation, AccountAuthorization accountAuthorization)
        {
            return _authorizationService.AuthorizeAsync(_userContext.User, accountAuthorization,
                new ResourceOperationRequirement(resourceOperation)).Result;
        }

        public void ChangePassword(UpdateUserPasswordDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.ChangePasswordAction.GetDescription()} action from AccountService. User Id: {userId}.");

            VerifyUserData(userId, dto.OldPassword, Literals.Literals.ChangePasswordAction.GetDescription(), out User user, dto.NewPassword);

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

            VerifyUserData(userId, dto.Password, Literals.Literals.DeleteUserAction.GetDescription(), out User user);

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
            return await _dbContext.UserClimbingSpotLinks.Where(x => x.UserId == userId).ToListAsync();
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

        private void VerifyUserData(int userId, string password, string operation, out User user, string newPassword = null)
        {
            var authorizationResult = Authorize(ResourceOperation.Delete, new AccountAuthorization() { UserId = userId });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            user = GetUserById(userId);
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
        }
    }
}
