using System.ComponentModel;
using HelpDeskSystem.Models;

namespace HelpDeskSystem.ViewModels
{
    public class TicketViewModel
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
        public int CategoryId { get; set; }

        [DisplayName("Ticket Sub-Category")]
        public int? SubCategoryId { get; set; }

        public TicketSubCategory SubCategory { get; set; }

        public List<Ticket> Tickets { get; set; }

        public Ticket TicketDetails { get; set; }

        public List<Comment> TicketComments { get; set; } // Menambahkan data TicketComments di TicketViewModel
        public Comment TicketComment { get; set; } // Menambahkan data TicketComment di TicketViewModel

        [DisplayName("Attachment")]
        public string Attachment { get; set; }
    }
}