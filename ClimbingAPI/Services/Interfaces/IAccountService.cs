using ClimbingAPI.Models.User;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void Register(CreateUserDto dto);
        Task<string> GenerateJwt(LoginUserDto dto);
        Task Update(UpdateUserDto dto, int userId);
        Task ChangePassword(UpdateUserPasswordDto dto, int userId);
        Task DeleteUser(DeleteUserDto dto, int userId);
    }
}
