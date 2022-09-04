using ClimbingAPI.Entities;
using ClimbingAPI.Utils;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityManager
{
    public class UserClimbingSpotManager
    {
        private readonly ClimbingDbContext _dbContext;
        public UserClimbingSpotManager(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task ManageUserClimbingSpotEntities(int climbingSpotId, int? userId)
        {
            var groupedByUserId = await _dbContext
                .UserClimbingSpotLinks
                .GroupBy(x => x.UserId)
                .Select(group => new
                {
                    Id = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            var userClimbingSpotLinks = await _dbContext.UserClimbingSpotLinks.Where(x => x.ClimbingSpotId == climbingSpotId).Select(x => new UserClimbingSpotLinks()
            { Id = x.Id, UserId = x.UserId}).ToListAsync();

            var userIdToRemove = new List<int>() { };
            var userToEdit = new List<int>() { };

            foreach(var user in userClimbingSpotLinks)
            {
                if (groupedByUserId.Where(x => x.Id == user.UserId).Select(x => x.Count).First() > 1)
                    userIdToRemove.Add(user.Id);
                else
                    userToEdit.Add(user.Id);
            }

            var userClimbingSpotLinksToDelete = _dbContext.UserClimbingSpotLinks.Where(x => userIdToRemove.Contains(x.Id));
            if(userIdToRemove.Count > 0)
                await LinqToDB.LinqExtensions.DeleteAsync(userClimbingSpotLinksToDelete.ToLinqToDB());

            var userClimbingSpotLinksToUpdate = _dbContext.UserClimbingSpotLinks.Where(x => userToEdit.Contains(x.Id));
            if (userToEdit.Count > 0)
                await LinqToDB.LinqExtensions.UpdateAsync(userClimbingSpotLinksToUpdate.ToLinqToDB(), x => new UserClimbingSpotLinks()
                {
                    ModifiedByUserId = userId.ToString(),
                    ModificationDateTime = DateTime.Now,
                    ClimbingSpotId = null
                }); 
        }
    }
}
