using ClimbingAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces
{
    public interface IAccountServiceGetDataHelper
    {
        Task<List<UserClimbingSpotLinks>> GetUserClimbingSpotsEntities(int userId);
        Task<User> GetUserById(int userId);
        Task<User> GetUserByEmail(string email);
    }
}
