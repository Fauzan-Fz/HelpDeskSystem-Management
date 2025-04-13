using System.ComponentModel;

namespace HelpDeskSystem.ViewModels
{
    public class RolesViewModel
    {
        [DisplayName("Role No")]
        public string Id { get; set; } // ID role

        [DisplayName("Role Name")]
        public string RoleName { get; set; } // Nama role
    }
}