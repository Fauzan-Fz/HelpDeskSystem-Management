using HelpDeskSystem.Models;

namespace HelpDeskSystem.Controllers
{
    public class TicketResolution : UserActivity
    {
        public int Id { get; set; }

        public string TicketId { get; set; }

        public Ticket Ticket { get; set; }

        public string Description { get; set; } // Untuk Keterangan Penyelesaian

        public int StatusId { get; set; } // Status Id froign key

        public SystemCodeDetail Status { get; set; } // Add Status
    }
}