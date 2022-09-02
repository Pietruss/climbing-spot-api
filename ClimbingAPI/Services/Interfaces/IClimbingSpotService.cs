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
        ClimbingSpotDto Get(int climbingSpotId);
        int Create(CreateClimbingSpotDto dto);
        void Delete(int climbingSpotId);
        void Update(UpdateClimbingSpotDto dto, int climbingSpotId);
        void AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto);
        Task<List<int>> GetClimbingSpotAssignedToUser(int userId);
        Task ValidateClimbingSpotById(int climbingSpotId);
        ClimbingSpot GetClimbingSpotWithAddressAndBouldersById(int climbingSpotId);
    }
}
