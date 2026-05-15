using Ticketing_system.Models;

namespace Ticketing_system.Service
{
    public class TicketService
    {
        public string GenerateTicketCode(int ticketId)
        {
            return $"TCK-{DateTime.Now.Year}-{ticketId.ToString("D4")}";
        }
    }
}
