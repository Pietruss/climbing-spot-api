using ClimbingAPI.Models.Boulder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IBoulderService
    {
        int Create(CreateBoulderModelDto dto, int climbingSpotId);
        BoulderDto Get(int climbingSpotId, int boulderId);
        Task<List<BoulderDto>> GetAll(int climbingSpotId);
        void Delete(int climbingSpotId, int boulderId);
        Task DeleteAll(int climbingSpotId);
        void Update(int climbingSpotId, int boulderId, UpdateBoulderDto dto);
    }
}
