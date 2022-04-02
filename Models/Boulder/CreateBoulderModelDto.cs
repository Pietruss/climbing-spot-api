using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Models.Boulder
{
    public class CreateBoulderModelDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
    }
}
