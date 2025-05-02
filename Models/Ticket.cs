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
        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }


        [DisplayName("Priority")]
        public int PriorityId { get; set; }
        public SystemCodeDetail Priority { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Ticket Category")]
        public int? SubCategoryId { get; set; }
        public TicketSubCategory SubCategory { get; set; }

        [DisplayName("Doucment Attachment")] // DisplayName untuk menampilkan label pada view
        public string Attachment { get; set; } // untuk upload file

        public ICollection<Comment> TicketComments { get; set; } // untuk relasi many to many dengan Comment 
    }
}
