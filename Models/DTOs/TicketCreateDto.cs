using System.ComponentModel.DataAnnotations;

public class TicketCreateDto
{
    [Required]
    public int AccountNumber { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Category { get; set; }

    public string? Title { get; set; }

    public string? PhoneNumber { get; set; } // optional

    
    [Required]
    public string Description { get; set; } // complaint

}