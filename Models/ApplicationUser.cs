using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Gender")]
        public int? GenderId { get; set; }

        public SystemCodeDetail Gender { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [DisplayName("Role")]
        public string? RoleId { get; set; }

        public IdentityRole Role { get; set; }
    }
}