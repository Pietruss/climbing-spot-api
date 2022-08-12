using System;
using System.ComponentModel.DataAnnotations;

namespace ClimbingAPI.Entities
{
    public class Role: IWhoColumnsEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
