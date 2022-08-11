using ClimbingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementClimbingSpotHandler: AuthorizationHandler<ResourceOperationRequirement, ClimbingSpot>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
            ClimbingSpot climbingSpot)
        {
            if (requirement.ResourceOperation == ResourceOperation.Create ||
                requirement.ResourceOperation == ResourceOperation.Read)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (climbingSpot.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
