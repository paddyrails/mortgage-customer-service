using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer.API.Models;

public class Employment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    public string EmployerName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? JobTitle { get; set; }

    [Required]
    public EmploymentType EmploymentType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AnnualIncome { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrent { get; set; } = true;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    // Navigation
    public Customer? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public int YearsEmployed => IsCurrent 
        ? DateTime.Today.Year - StartDate.Year 
        : (EndDate?.Year ?? DateTime.Today.Year) - StartDate.Year;
}

public enum EmploymentType
{
    FullTime = 1,
    PartTime = 2,
    SelfEmployed = 3,
    Retired = 4,
    Unemployed = 5,
    Contract = 6
}
