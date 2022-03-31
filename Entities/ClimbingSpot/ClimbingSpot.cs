using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Entities
{
    public class ClimbingSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public int AddressId { get; set; }
        public virtual Address.Address Address { get; set; }
        public virtual List<Boulder.Boulder> Boulder { get; set; }
        public string Author { get; set; }
        public DateTime ModificationTime { get; set; }

    }
}
