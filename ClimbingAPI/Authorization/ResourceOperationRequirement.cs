using Microsoft.AspNetCore.Authorization;

namespace ClimbingAPI.Authorization
{
    public class ResourceOperationRequirement: IAuthorizationRequirement
    {

        public ResourceOperationRequirement(ResourceOperation resourceOperation)
        {
            ResourceOperation = resourceOperation;
        }

        public ResourceOperation ResourceOperation { get; }
    }
}
