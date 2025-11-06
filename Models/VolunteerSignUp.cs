using System.ComponentModel.DataAnnotations;

namespace VolunteerSignUpSystem.Models;

public class VolunteerSignUp
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Interests")]
    public string Interests { get; set; } = string.Empty;

    [Display(Name = "Availability")]
    public string Availability { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

