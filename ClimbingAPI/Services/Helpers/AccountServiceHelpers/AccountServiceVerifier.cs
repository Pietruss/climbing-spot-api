using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers
{
    public class AccountServiceVerifier: IAccountServiceVerifier
    {
        private readonly ILogger<AccountServiceVerifier> _logger;
        private readonly IClimbingSpotService _climbingSpotService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContext;
        private readonly IAccountServiceGetDataHelper _accountServiceGetDataHelper;

        public AccountServiceVerifier(IClimbingSpotService climbingSpotService, ILogger<AccountServiceVerifier> logger, IPasswordHasher<User> passwordHasher, IAuthorizationService authorizationService, IUserContextService userContext, IAccountServiceGetDataHelper accountServiceGetDataHelper)
        {
            _logger = logger;
            _climbingSpotService = climbingSpotService;
            _passwordHasher = passwordHasher;
            _authorizationService = authorizationService;
            _userContext = userContext;
            _accountServiceGetDataHelper = accountServiceGetDataHelper;
        }
        public async Task VerifyIfAnyClimbingSpotIsAssignedToUser(int userId)
        {
            var climbingSpots = await _climbingSpotService.GetClimbingSpotAssignedToUser(userId);
            if (climbingSpots.Any())
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UserAssignedToClimbingSpot.GetDescription()} action from AccountService. User is assigned to Climbing Spot.");
                throw new BadRequestException(Literals.Literals.UserAssignedToClimbingSpot.GetDescription());
            }
        }

        public async Task<User> VerifyUserData(int userId, string password, string operation, string newPassword = null)
        {
            var authorizationResult = Authorize(ResourceOperation.Delete, new AccountAuthorization() { UserId = userId });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from AccountService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var user = await _accountServiceGetDataHelper.GetUserById(userId);
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

            if (operation == Literals.Literals.ChangePasswordAction.GetDescription())
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

        public void VerifyHashedPassword(LoginUserDto dto, User user)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogError("Invalid username or password.");
                throw new BadRequestException(Literals.Literals.InvalidUsernameOrPassowrd.GetDescription());
            }
        }

        public PasswordVerificationResult IsTheSamePassword(User user, string oldPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
        }

        public AuthorizationResult Authorize(ResourceOperation resourceOperation, AccountAuthorization accountAuthorization)
        {
            return _authorizationService.AuthorizeAsync(_userContext.User, accountAuthorization,
                new ResourceOperationRequirement(resourceOperation)).Result;
        }
    }
}
