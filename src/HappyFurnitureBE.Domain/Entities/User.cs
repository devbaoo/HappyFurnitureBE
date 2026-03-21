using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "user"; // default role
}