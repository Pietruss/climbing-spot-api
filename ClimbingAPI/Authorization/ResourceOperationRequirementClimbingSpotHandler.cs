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
    public class ResourceOperationRequirementClimbingSpotHandler: AuthorizationHandler<ResourceOperationRequirement, ClimbingSpotAuthorization>
    {
        private readonly ClimbingDbContext _dbContext;
        public ResourceOperationRequirementClimbingSpotHandler(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async  Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
            ClimbingSpotAuthorization climbingSpot)
        {
            var userId = int.Parse(context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (requirement.ResourceOperation == ResourceOperation.Create)
            {
                var userClimbingSpot = await _dbContext
                    .UserClimbingSpotLinks
                    .Where(x =>
                        x.UserId == userId && (x.RoleId == (int)Roles.Admin || x.RoleId == (int)Roles.Manager))
                    .Select(x => new UserClimbingSpotLinks() { Id = x.Id})
                    .FirstOrDefaultAsync();
                if (userClimbingSpot is null)
                    return Task.CompletedTask;

                context.Succeed(requirement);
            }

            if(requirement.ResourceOperation == ResourceOperation.Delete || requirement.ResourceOperation == ResourceOperation.Update)
            {
                if (climbingSpot.CreatedById != userId)
                    return Task.CompletedTask;

                context.Succeed(requirement);
            }


            return Task.CompletedTask;
        }
    }
}
