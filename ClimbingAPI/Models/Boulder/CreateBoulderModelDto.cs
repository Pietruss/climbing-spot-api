﻿namespace ClimbingAPI.Models.Boulder
{
    public class CreateBoulderModelDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public int CreatedById { get; set; }
    }
}
