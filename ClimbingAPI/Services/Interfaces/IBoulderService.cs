using ClimbingAPI.Models.Boulder;
using System.Collections.Generic;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IBoulderService
    {
        int Create(CreateBoulderModelDto dto, int climbingSpotId);
        BoulderDto Get(int climbingSpotId, int boulderId);
        List<BoulderDto> GetAll(int climbingSpotId);
        void Delete(int climbingSpotId, int boulderId);
        void DeleteAll(int climbingSpotId);
        void Update(int climbingSpotId, int boulderId, UpdateBoulderDto dto);
    }
}
