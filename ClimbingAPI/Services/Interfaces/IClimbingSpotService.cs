using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using System.Collections.Generic;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IClimbingSpotService
    {
        IEnumerable<ClimbingSpotDto> GetAll();
        ClimbingSpotDto Get(int id);
        int Create(CreateClimbingSpotDto dto);
        void Delete(int id);
        void Update(UpdateClimbingSpotDto dto, int id);
        void AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto);
    }
}
