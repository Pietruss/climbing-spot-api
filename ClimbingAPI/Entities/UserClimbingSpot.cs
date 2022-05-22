using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Entities
{
    public class UserClimbingSpot
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ClimbingSpotId { get; set; }
        public int? RoleId { get; set; }
    }
}
