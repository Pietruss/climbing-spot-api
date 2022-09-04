using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IClimbingSpotService
    {
        Task<IEnumerable<ClimbingSpotDto>> GetAll();
        Task<ClimbingSpotDto> Get(int climbingSpotId);
        Task<int> Create(CreateClimbingSpotDto dto);
        Task Delete(int climbingSpotId);
        void Update(UpdateClimbingSpotDto dto, int climbingSpotId);
        Task AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto);
        Task<List<int>> GetClimbingSpotAssignedToUser(int userId);
        Task ValidateClimbingSpotById(int climbingSpotId);
        Task<ClimbingSpot> GetClimbingSpotWithAddressAndBouldersById(int climbingSpotId);
    }
}
