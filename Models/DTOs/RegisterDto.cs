using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ticketing_system.Models.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int DepartmentId { get; set; }

        public List<SelectListItem>? AvailableDepartments { get; set; }
    }
}
