using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.User;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void Register(CreateUserDto dto);
        string GenerateJwt(LoginUserDto dto);
    }
}
