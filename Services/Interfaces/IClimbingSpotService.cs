using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        int Create(CreateClimbingSpotDto dto, int userId);
        void Delete(int id, ClaimsPrincipal user);
        void Update(UpdateClimbingSpotDto dto, int id, ClaimsPrincipal user);
    }
}
