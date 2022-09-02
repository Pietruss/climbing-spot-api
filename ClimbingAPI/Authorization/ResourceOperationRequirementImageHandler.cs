using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementImageHandler : AuthorizationHandler<ResourceOperationRequirement, ImageAuthorization>
    {
        private readonly ClimbingDbContext _dbContext;
        public ResourceOperationRequirementImageHandler(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, ImageAuthorization imageAuthorization)
        {
            var userId = int.Parse(context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (requirement.ResourceOperation == ResourceOperation.Create || requirement.ResourceOperation == ResourceOperation.Delete) 
            {
                var userClimbingSpot = await _dbContext.UserClimbingSpotLinks
                    .Where(x => x.UserId == userId && (x.RoleId == (int)Roles.Admin || x.RoleId == (int)Roles.Manager || x.RoleId == (int)Roles.Routesetter) && x.ClimbingSpotId == imageAuthorization.ClimbingSpotId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                if (userClimbingSpot == 0)
                    return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
