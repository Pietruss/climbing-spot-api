using ClimbingAPI.Models.Boulder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IBoulderService
    {
        Task<int> Create(CreateBoulderModelDto dto, int climbingSpotId);
        Task<BoulderDto> Get(int climbingSpotId, int boulderId);
        Task<List<BoulderDto>> GetAll(int climbingSpotId);
        Task Delete(int climbingSpotId, int boulderId);
        Task DeleteAll(int climbingSpotId);
        Task Update(int climbingSpotId, int boulderId, UpdateBoulderDto dto);
    }
}
