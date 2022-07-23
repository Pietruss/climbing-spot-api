using ClimbingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementBoulderHandler: AuthorizationHandler<ResourceOperationRequirement, BoulderUpdate>
    {
        private readonly ClimbingDbContext _dbContext;
        public ResourceOperationRequirementBoulderHandler(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            BoulderUpdate boulderUpdate)
        {
            if (requirement.ResourceOperation == ResourceOperation.Create ||
                requirement.ResourceOperation == ResourceOperation.Read || requirement.ResourceOperation == ResourceOperation.Delete)
            {
                context.Succeed(requirement);
            }

            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderUpdate.BoulderId);
            if (boulder is null || boulder.ClimbingSpotId != boulderUpdate.ClimbingSpotId)
                return Task.CompletedTask;

            var userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userClimbingSpotEntity = _dbContext.UserClimbingSpotLinks.FirstOrDefault(x =>
                 x.UserId == int.Parse(userId) && x.ClimbingSpotId == boulderUpdate.ClimbingSpotId && (x.RoleId == 1 ||
                 x.RoleId == 2 || x.RoleId == 3));
            if (userClimbingSpotEntity is null)
                return Task.CompletedTask;

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
