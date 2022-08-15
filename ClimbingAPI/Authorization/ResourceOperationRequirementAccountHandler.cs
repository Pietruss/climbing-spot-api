using ClimbingAPI.Authorization.AuthorizationEntity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirementAccountHandler : AuthorizationHandler<ResourceOperationRequirement, AccountAuthorization>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, AccountAuthorization accountAuthorization)
        {
            var userId = int.Parse(context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);
            if (requirement.ResourceOperation == ResourceOperation.Update || requirement.ResourceOperation == ResourceOperation.Delete)
            {
                if (userId != accountAuthorization.UserId)
                    return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
