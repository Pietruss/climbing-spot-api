using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Identity;
using System;

namespace ClimbingAPI.Services.Helpers
{
    public class AccountServiceUpdateHelper: IAccountServiceUpdateHelper
    {
        private readonly ClimbingDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountServiceUpdateHelper(ClimbingDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }
        public void UpdateUserPassword(UpdateUserPasswordDto dto, User user)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            WhoColumns.ModificationFiller(user, user.Id, DateTime.Now);

            _dbContext.User.Update(user);
            _dbContext.SaveChanges();
        }

        public User UpdateUser(User user, UpdateUserDto dto)
        {
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DateOfBirth = dto.DateOfBirth;

            return user;
        }
    }
}
