using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClimbingAPI.Entities.Address
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public virtual ClimbingSpot ClimbingSpot { get; set; }
        public string Author { get; set; }
        public DateTime ModificationTime { get; set; }
    }
}
