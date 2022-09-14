using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces
{
    public interface IAccountServiceVerifier
    {
        Task VerifyIfAnyClimbingSpotIsAssignedToUser(int userId);
        Task<User> VerifyUserData(int userId, string password, string operation, string newPassword = null);
        void VerifyHashedPassword(LoginUserDto dto, User user);
        PasswordVerificationResult IsTheSamePassword(User user, string oldPassword);
        AuthorizationResult Authorize(ResourceOperation resourceOperation, AccountAuthorization accountAuthorization);
    }
}
