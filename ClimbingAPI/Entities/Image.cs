using System;

namespace ClimbingAPI.Entities
{
    public class Image: IWhoColumnsEntity
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }
        public Boulder.Boulder Boulder { get; set; }
        public int BoulderId { get; set; }
        public string ModifiedByUserId { get ; set; }
        public DateTime ModificationDateTime { get ; set;  }
        public string CreatedByUserId { get ; set;  }
        public DateTime CreationDateTime { get ; set;  }
    }
}
