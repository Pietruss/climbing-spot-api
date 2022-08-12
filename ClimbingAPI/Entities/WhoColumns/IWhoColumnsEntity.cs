using System;

namespace ClimbingAPI.Entities
{
    public interface IWhoColumnsEntity
    {
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
