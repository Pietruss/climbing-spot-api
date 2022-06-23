using ClimbingAPI.Entities.Boulder;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementBoulderHandler: AuthorizationHandler<ResourceOperationRequirement, Boulder>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            Boulder boulder)
        {
            if (requirement.ResourceOperation == ResourceOperation.Create ||
                requirement.ResourceOperation == ResourceOperation.Read || requirement.ResourceOperation == ResourceOperation.Delete)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (boulder.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
