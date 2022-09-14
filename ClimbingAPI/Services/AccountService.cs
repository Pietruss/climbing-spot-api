using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace ClimbingAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly ClimbingDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAccountServiceGetDataHelper _accountServiceGetDataHelper;
        private readonly IAccountServiceUpdateHelper _accountServiceUpdateHelper;
        private readonly IAccountServiceJwtHelper _accountServiceJwtHelper;
        private readonly IAccountServiceVerifier _accountServiceVerifier;
        private readonly IAccountServiceCreateAndRemoveHelper _accountServiceCreateAndRemoveHelper;

        public AccountService()
        {
            
        }
        public AccountService(IMapper mapper, ILogger<AccountService> logger, ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher, IAccountServiceGetDataHelper accountServiceGetDataHelper, IAccountServiceUpdateHelper accountServiceUpdateHelper, IAccountServiceJwtHelper accountServiceJwtHelper, IAccountServiceVerifier accountServiceVerifier, IAccountServiceCreateAndRemoveHelper accountServiceCreateAndRemoveHelper)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _accountServiceGetDataHelper = accountServiceGetDataHelper;
            _accountServiceUpdateHelper = accountServiceUpdateHelper;
            _accountServiceJwtHelper = accountServiceJwtHelper;
            _accountServiceVerifier = accountServiceVerifier;
            _accountServiceCreateAndRemoveHelper = accountServiceCreateAndRemoveHelper;
        }
        public void Register(CreateUserDto dto)
        {
            _logger.LogInformation("INFO for: REGISTER action from AccountService.");

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            WhoColumns.CreationFiller(user, user.Id, DateTime.Now);

            var userClimbingSpot = _accountServiceCreateAndRemoveHelper.CreateUserClimbingSpotEntity(user.Id, dto.RoleId);

            WhoColumns.CreationFiller(userClimbingSpot, user.Id, DateTime.Now);
            _dbContext.UserClimbingSpotLinks.Add(userClimbingSpot);
            _dbContext.SaveChanges();
        }

        public async Task<string> Login(LoginUserDto dto)
        {
            _logger.LogInformation("INFO for: GENERATEJWT action from AccountService.");

            var user = await _accountServiceGetDataHelper.GetUserByEmail(dto.Email);
            if (user is null)
            {
                _logger.LogError("Invalid username or password.");
                throw new BadRequestException(Literals.Literals.InvalidUsernameOrPassowrd.GetDescription());
            }

            _accountServiceVerifier.VerifyHashedPassword(dto, user);

            var token = _accountServiceJwtHelper.GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        
        public async Task Update(UpdateUserDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: UPDATE action from AccountService. User Id: {userId}.");

            var authorizationResult = _accountServiceVerifier.Authorize(ResourceOperation.Update, new AccountAuthorization() { UserId =  userId});
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: UPDATE action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var user = await _accountServiceGetDataHelper.GetUserById(userId);
            if(user is null)
            {
                _logger.LogError($"ERROR for: UPDATE action from ClimbingSpotService. User with id not found {userId}.");
                throw new NotFoundException($"User with id not found {userId}.");
            }

            user = _accountServiceUpdateHelper.UpdateUser(user, dto);
            WhoColumns.ModificationFiller(user, userId, DateTime.Now);

            _dbContext.Update(user);
            _dbContext.SaveChanges();
        }

        public async Task ChangePassword(UpdateUserPasswordDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.ChangePasswordAction.GetDescription()} action from AccountService. User Id: {userId}.");

            var user = await _accountServiceVerifier.VerifyUserData(userId, dto.OldPassword, Literals.Literals.ChangePasswordAction.GetDescription(), dto.NewPassword);

            _accountServiceUpdateHelper.UpdateUserPassword(dto, user);
        }

        public async Task DeleteUser(DeleteUserDto dto, int userId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.DeleteUserAction.GetDescription()} action from AccountService. User Id: {userId}.");

            var user = await _accountServiceVerifier.VerifyUserData(userId, dto.Password, Literals.Literals.DeleteUserAction.GetDescription());

            await _accountServiceVerifier.VerifyIfAnyClimbingSpotIsAssignedToUser(userId);

            await _accountServiceCreateAndRemoveHelper.RemoveUserAndUserClimbingSpotEntites(user, userId);
        }
    }
}
