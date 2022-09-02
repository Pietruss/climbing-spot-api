using System;

namespace ClimbingAPI.Models.Boulder
{
    public class BoulderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime ModificationTime { get; set; }
    }
}
