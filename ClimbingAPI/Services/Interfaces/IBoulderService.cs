using ClimbingAPI.Models.Boulder;
using System.Collections.Generic;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IBoulderService
    {
        int Create(CreateBoulderModelDto dto, int climbingSpotId);
        BoulderDto Get(int boulderId, int climbingSpotId);
        List<BoulderDto> GetAll(int climbingSpotId);
        void Delete(int boulderId, int climbingSpotId);
        void DeleteAll(int climbingSpotId);
        void Update(int boulderId, int climbingSpotId, UpdateBoulderDto dto);
    }
}
