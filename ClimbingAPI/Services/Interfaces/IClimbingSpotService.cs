﻿using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using System.Collections.Generic;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IClimbingSpotService
    {
        IEnumerable<ClimbingSpotDto> GetAll();
        ClimbingSpotDto Get(int climbingSpotId);
        int Create(CreateClimbingSpotDto dto);
        void Delete(int climbingSpotId);
        void Update(UpdateClimbingSpotDto dto, int climbingSpotId);
        void AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto);
    }
}
