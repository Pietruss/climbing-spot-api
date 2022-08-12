using System;
using System.ComponentModel.DataAnnotations;

namespace ClimbingAPI.Entities
{
    public class User: IWhoColumnsEntity
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
        public string ModifiedByUserId { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
}
