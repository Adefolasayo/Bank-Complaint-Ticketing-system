namespace Ticketing_system.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; }

        public string OldStatus { get; set; } = "Open";

        public string NewStatus { get; set; } = "Assigned";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
