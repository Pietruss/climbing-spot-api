using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces
{
    public interface IAccountServiceUpdateHelper
    {
        void UpdateUserPassword(UpdateUserPasswordDto dto, User user);
        User UpdateUser(User user, UpdateUserDto dto);
    }
}
