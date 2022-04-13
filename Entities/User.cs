using System;
using System.ComponentModel.DataAnnotations;

namespace ClimbingAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PasswordHash { get; set; }
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }

    }
}
