using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.ClimbingSpot;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IClimbingSpotService
    {
        IEnumerable<ClimbingSpotDto> GetAll();
        ClimbingSpotDto Get(int id);
        int Create(CreateClimbingSpotDto dto);
        void Delete(int id);
        void Update(UpdateClimbingSpotDto dto, int id);
    }
}
