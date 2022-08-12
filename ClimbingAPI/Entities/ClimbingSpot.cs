using System;
using System.Collections.Generic;

namespace ClimbingAPI.Entities
{
    public class ClimbingSpot: IWhoColumnsEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public int AddressId { get; set; }
        public virtual Address.Address Address { get; set; }
        public virtual List<Boulder.Boulder> Boulder { get; set; }
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
}
