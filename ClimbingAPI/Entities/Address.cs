using System;

namespace ClimbingAPI.Entities.Address
{
    public class Address: IWhoColumnsEntity
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public virtual ClimbingSpot ClimbingSpot { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
