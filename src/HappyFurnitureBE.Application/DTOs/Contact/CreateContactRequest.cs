using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Application.DTOs.Contact;

public class CreateContactRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject is required")]
    [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "reCAPTCHA token is required")]
    public string RecaptchaToken { get; set; } = string.Empty;
}
