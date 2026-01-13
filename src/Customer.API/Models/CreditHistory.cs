using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer.API.Models;

public class CreditHistory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [Range(300, 850)]
    public int CreditScore { get; set; }

    [Required]
    public DateTime ReportDate { get; set; }

    [StringLength(50)]
    public string CreditBureau { get; set; } = "Experian";

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDebt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableCredit { get; set; }

    public int NumberOfAccounts { get; set; }

    public int LatePayments { get; set; }

    public int Bankruptcies { get; set; }

    public int Foreclosures { get; set; }

    // Navigation
    public Customer? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public string CreditRating => CreditScore switch
    {
        >= 800 => "Excellent",
        >= 740 => "Very Good",
        >= 670 => "Good",
        >= 580 => "Fair",
        _ => "Poor"
    };

    [NotMapped]
    public decimal DebtToIncomeRatio => AvailableCredit > 0 
        ? Math.Round(TotalDebt / AvailableCredit * 100, 2) 
        : 0;
}
