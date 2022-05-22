using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void Register(CreateUserDto dto);
        string GenerateJwt(LoginUserDto dto);
        void AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto);
    }
}
