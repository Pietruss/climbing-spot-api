﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClimbingAPI
{
    public class UserRoleSeeder
    {
        private readonly ClimbingDbContext _dbContext;

        public UserRoleSeeder(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Database.IsRelational())
                {
                    if (!_dbContext.Role.Any())
                    {
                        _dbContext.Role.AddRange(GetRoles());
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "Admin"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Bald Builder"
                },
                new Role()
                {
                    Name = "User"
                }
            };

            return roles;
        }
    }
}
