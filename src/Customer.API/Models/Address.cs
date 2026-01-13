using System.ComponentModel.DataAnnotations;

namespace Customer.API.Models;

public class Address
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Unit { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(50)]
    public string Country { get; set; } = "USA";

    public AddressType AddressType { get; set; } = AddressType.Primary;

    // Navigation
    public Customer? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum AddressType
{
    Primary = 1,
    Mailing = 2,
    Previous = 3
}
