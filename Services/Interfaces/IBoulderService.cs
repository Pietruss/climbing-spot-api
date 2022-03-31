using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.Boulder;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IBoulderService
    {
        int Create(CreateBoulderModelDto dto, int climbingSpotId);
        BoulderDto Get(int boulderId, int climbingSpotId);
        List<BoulderDto> GetAll(int climbingSpotId);
    }
}
