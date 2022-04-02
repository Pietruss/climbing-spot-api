using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ClimbingAPI.Services
{
    public class AccountService: IAccountService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly ClimbingDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(IMapper mapper, ILogger<AccountService> logger, ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }
        public void Register(CreateUserDto dto)
        {
            _logger.LogInformation("INFO for: REGISTER action from AccountService.");

            var user = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = dto.RoleId
            };

            var hashedPassword =_passwordHasher.HashPassword(user, dto.Password);
            user.PasswordHash = hashedPassword;

            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
        }
    }
}
