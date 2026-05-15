using System.ComponentModel.DataAnnotations;

namespace Ticketing_system.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public int AccountNumber { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; }
    }
}
