namespace Ticketing_system.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // Navigation
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
