namespace HelpDeskSystem.Models
{
    public class UserActivity
    {
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
