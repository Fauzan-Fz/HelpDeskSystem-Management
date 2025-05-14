using Microsoft.AspNetCore.Identity;

namespace HelpDeskSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        public string? RoleId { get; set; }
        public IdentityRole Role { get; set; }
    }
}