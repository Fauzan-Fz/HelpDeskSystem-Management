namespace HelpDeskSystem.Models
{
    public class TicketCategory : UserActivity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
