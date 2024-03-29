﻿using System;

namespace ClimbingAPI.Entities
{
    public class UserClimbingSpotLinks: IWhoColumnsEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ClimbingSpotId { get; set; }
        public int? RoleId { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
