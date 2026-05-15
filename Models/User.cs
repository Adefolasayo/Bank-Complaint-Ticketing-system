using System.ComponentModel.DataAnnotations;

namespace Ticketing_system.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // Agent, Staff, Admin

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Ticket> AssignedTickets { get; set; }
    }
}
