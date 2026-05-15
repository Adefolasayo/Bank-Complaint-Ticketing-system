using System.ComponentModel.DataAnnotations;
using Ticketing_system.Models.Enums;

namespace Ticketing_system.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string? TicketCode { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        // Customer رابطه
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Department
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        // Assigned Staff
        public int? AssignedToUserId { get; set; }
        public User AssignedToUser { get; set; }

        public TicketStatus Status { get; set; }
        public string Priority { get; set; } = "Medium";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Navigation
        public ICollection<TicketComment> Comments { get; set; }
    }
}
