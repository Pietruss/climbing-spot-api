using ClimbingAPI.Entities;
using ClimbingAPI.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementBoulderHandler: AuthorizationHandler<ResourceOperationRequirement, BoulderAuthorization>
    {
        private readonly ClimbingDbContext _dbContext;
        public ResourceOperationRequirementBoulderHandler(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            BoulderAuthorization boulderAuthorization)
        {
            var userClimbingSpotEntity = GetUserClimbingSpotLinks(context, boulderAuthorization);

            if (requirement.ResourceOperation == ResourceOperation.Delete || requirement.ResourceOperation == ResourceOperation.Create || requirement.ResourceOperation == ResourceOperation.Update)
            {
                if (userClimbingSpotEntity is null)
                    return Task.CompletedTask;

                if(requirement.ResourceOperation == ResourceOperation.Update)
                {
                    var climbingSpot = _dbContext.ClimbingSpot.Include(x => x.Boulder).FirstOrDefault(x => x.Id == boulderAuthorization.ClimbingSpotId);
                    if(climbingSpot is null || climbingSpot.Boulder is null)
                        return Task.CompletedTask;

                    var boulder = climbingSpot.Boulder.FirstOrDefault(x => x.Id == boulderAuthorization.BoulderId);
                    if (boulder is null)
                        return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private UserClimbingSpotLinks GetUserClimbingSpotLinks(AuthorizationHandlerContext context, BoulderAuthorization boulderAuthorization)
        {
            var userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return  _dbContext.UserClimbingSpotLinks.FirstOrDefault(x =>
                 x.UserId == int.Parse(userId) && x.ClimbingSpotId == boulderAuthorization.ClimbingSpotId && (x.RoleId == (int)Roles.Admin ||
                 x.RoleId == (int)Roles.Manager || x.RoleId == (int)Roles.Routesetter));
        }
    }
}
