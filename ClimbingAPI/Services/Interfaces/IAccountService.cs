using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void Register(CreateUserDto dto);
        string GenerateJwt(LoginUserDto dto);
    }
}
