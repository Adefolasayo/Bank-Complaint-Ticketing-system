using System.ComponentModel.DataAnnotations;

namespace Ticketing_system.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Comment { get; set; }

        public bool IsInternal { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
