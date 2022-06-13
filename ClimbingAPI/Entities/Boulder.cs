using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Entities.Boulder
{
    public class Boulder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public int ClimbingSpotId { get; set; }
        public string Author { get; set; }
        public DateTime ModificationTime { get; set; }
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }

    }
}
