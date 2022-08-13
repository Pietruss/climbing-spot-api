using ClimbingAPI.Models.User;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void Register(CreateUserDto dto);
        string GenerateJwt(LoginUserDto dto);
        void Update(UpdateUserDto dto, int userId);
        void ChangePassword(UpdateUserPasswordDto dto, int userId);
    }
}
