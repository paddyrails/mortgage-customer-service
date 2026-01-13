using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer.API.Models;

public class Customer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(11)]
    public string SSN { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    // Navigation properties
    public Address? Address { get; set; }
    public List<Employment> Employments { get; set; } = new();
    public CreditHistory? CreditHistory { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public int Age => DateTime.Today.Year - DateOfBirth.Year - 
        (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}
