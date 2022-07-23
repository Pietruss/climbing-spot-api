namespace ClimbingAPI.Entities
{
    public class UserClimbingSpotLinks
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ClimbingSpotId { get; set; }
        public int? RoleId { get; set; }
    }
}
