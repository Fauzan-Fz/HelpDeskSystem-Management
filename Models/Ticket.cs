using System.ComponentModel;

namespace HelpDeskSystem.Models
{
    public class Ticket
    {
        [DisplayName("Id")]
        public string Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Priority")]
        public string Priority { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Ticket Category")]
        public int? SubCategoryId { get; set; }
        public TicketSubCategory SubCategory { get; set; }
    }
}
