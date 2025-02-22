namespace HelpDeskSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TicketId { get; set; } // Changed from int to string
        public Ticket Ticket { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
    }
}
