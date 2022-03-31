using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Models.ClimbingSpot
{
    public class UpdateClimbingSpotDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [EmailAddress]
        public string ContactEmail { get; set; }
        [Phone]
        public string ContactNumber { get; set; }
    }
}
