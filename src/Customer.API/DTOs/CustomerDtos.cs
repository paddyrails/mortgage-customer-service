using System.ComponentModel.DataAnnotations;
using Customer.API.Models;

namespace Customer.API.DTOs;

// Response DTO
public record CustomerResponseDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public int Age { get; init; }
    public AddressResponseDto? Address { get; init; }
    public List<EmploymentResponseDto> Employments { get; init; } = new();
    public CreditResponseDto? CreditHistory { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

// Create DTO
public record CreateCustomerDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [StringLength(11, MinimumLength = 9)]
    public string SSN { get; init; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; init; }

    public CreateAddressDto? Address { get; init; }
}

// Update DTO
public record UpdateCustomerDto
{
    [StringLength(50, MinimumLength = 2)]
    public string? FirstName { get; init; }

    [StringLength(50, MinimumLength = 2)]
    public string? LastName { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    [Phone]
    public string? Phone { get; init; }

    public UpdateAddressDto? Address { get; init; }
}

// Address DTOs
public record AddressResponseDto
{
    public Guid Id { get; init; }
    public string Street { get; init; } = string.Empty;
    public string? Unit { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string FullAddress => string.IsNullOrEmpty(Unit) 
        ? $"{Street}, {City}, {State} {ZipCode}" 
        : $"{Street} {Unit}, {City}, {State} {ZipCode}";
}

public record CreateAddressDto
{
    [Required]
    [StringLength(200)]
    public string Street { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Unit { get; init; }

    [Required]
    [StringLength(100)]
    public string City { get; init; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string State { get; init; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string ZipCode { get; init; } = string.Empty;

    [StringLength(50)]
    public string Country { get; init; } = "USA";
}

public record UpdateAddressDto
{
    public string? Street { get; init; }
    public string? Unit { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
}

// Employment DTOs
public record EmploymentResponseDto
{
    public Guid Id { get; init; }
    public string EmployerName { get; init; } = string.Empty;
    public string? JobTitle { get; init; }
    public string EmploymentType { get; init; } = string.Empty;
    public decimal AnnualIncome { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public int YearsEmployed { get; init; }
}

public record CreateEmploymentDto
{
    [Required]
    [StringLength(100)]
    public string EmployerName { get; init; } = string.Empty;

    [StringLength(100)]
    public string? JobTitle { get; init; }

    [Required]
    public EmploymentType EmploymentType { get; init; }

    [Required]
    [Range(0, 100000000)]
    public decimal AnnualIncome { get; init; }

    [Required]
    public DateTime StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public bool IsCurrent { get; init; } = true;
}

// Credit DTOs
public record CreditResponseDto
{
    public Guid Id { get; init; }
    public int CreditScore { get; init; }
    public string CreditRating { get; init; } = string.Empty;
    public DateTime ReportDate { get; init; }
    public string CreditBureau { get; init; } = string.Empty;
    public decimal TotalDebt { get; init; }
    public decimal AvailableCredit { get; init; }
    public decimal DebtToIncomeRatio { get; init; }
    public int NumberOfAccounts { get; init; }
    public int LatePayments { get; init; }
}

public record UpdateCreditDto
{
    [Required]
    [Range(300, 850)]
    public int CreditScore { get; init; }

    public decimal? TotalDebt { get; init; }
    public decimal? AvailableCredit { get; init; }
    public int? NumberOfAccounts { get; init; }
    public int? LatePayments { get; init; }
}

// API Response wrapper
public record ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public List<string>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> FailResponse(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
