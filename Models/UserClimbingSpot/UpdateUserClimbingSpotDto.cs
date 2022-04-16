using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Models.UserClimbingSpot
{
    public class UpdateUserClimbingSpotDto
    {
        public int UserId { get; set; }
        public int ClimbingSpotId { get; set; }
        public int RoleId { get; set; }
    }
}
