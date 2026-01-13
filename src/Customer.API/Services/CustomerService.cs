using Microsoft.EntityFrameworkCore;
using Customer.API.Data;
using Customer.API.DTOs;
using Customer.API.Models;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(CustomerDbContext context, ILogger<CustomerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.Address)
            .Include(c => c.Employments)
            .Include(c => c.CreditHistory)
            .Where(c => c.IsActive)
            .ToListAsync();

        return customers.Select(MapToResponseDto);
    }

    public async Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.Address)
            .Include(c => c.Employments)
            .Include(c => c.CreditHistory)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        return customer == null ? null : MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> GetCustomerByEmailAsync(string email)
    {
        var customer = await _context.Customers
            .Include(c => c.Address)
            .Include(c => c.Employments)
            .Include(c => c.CreditHistory)
            .FirstOrDefaultAsync(c => c.Email == email && c.IsActive);

        return customer == null ? null : MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Models.Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            SSN = dto.SSN,
            DateOfBirth = dto.DateOfBirth
        };

        if (dto.Address != null)
        {
            customer.Address = new Address
            {
                CustomerId = customer.Id,
                Street = dto.Address.Street,
                Unit = dto.Address.Unit,
                City = dto.Address.City,
                State = dto.Address.State,
                ZipCode = dto.Address.ZipCode,
                Country = dto.Address.Country
            };
        }

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created customer: {CustomerId}", customer.Id);

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (customer == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            customer.FirstName = dto.FirstName;
        if (!string.IsNullOrWhiteSpace(dto.LastName))
            customer.LastName = dto.LastName;
        if (!string.IsNullOrWhiteSpace(dto.Email))
            customer.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Phone))
            customer.Phone = dto.Phone;

        if (dto.Address != null && customer.Address != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Address.Street))
                customer.Address.Street = dto.Address.Street;
            if (dto.Address.Unit != null)
                customer.Address.Unit = dto.Address.Unit;
            if (!string.IsNullOrWhiteSpace(dto.Address.City))
                customer.Address.City = dto.Address.City;
            if (!string.IsNullOrWhiteSpace(dto.Address.State))
                customer.Address.State = dto.Address.State;
            if (!string.IsNullOrWhiteSpace(dto.Address.ZipCode))
                customer.Address.ZipCode = dto.Address.ZipCode;
            customer.Address.UpdatedAt = DateTime.UtcNow;
        }

        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated customer: {CustomerId}", customer.Id);

        return MapToResponseDto(customer);
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        customer.IsActive = false;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted customer: {CustomerId}", id);

        return true;
    }

    public async Task<EmploymentResponseDto> AddEmploymentAsync(Guid customerId, CreateEmploymentDto dto)
    {
        var employment = new Employment
        {
            CustomerId = customerId,
            EmployerName = dto.EmployerName,
            JobTitle = dto.JobTitle,
            EmploymentType = dto.EmploymentType,
            AnnualIncome = dto.AnnualIncome,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = dto.IsCurrent
        };

        _context.Employments.Add(employment);
        await _context.SaveChangesAsync();

        return MapEmploymentToDto(employment);
    }

    public async Task<IEnumerable<EmploymentResponseDto>> GetEmploymentsAsync(Guid customerId)
    {
        var employments = await _context.Employments
            .Where(e => e.CustomerId == customerId)
            .OrderByDescending(e => e.IsCurrent)
            .ThenByDescending(e => e.StartDate)
            .ToListAsync();

        return employments.Select(MapEmploymentToDto);
    }

    public async Task<CreditResponseDto?> GetCreditHistoryAsync(Guid customerId)
    {
        var credit = await _context.CreditHistories
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        return credit == null ? null : MapCreditToDto(credit);
    }

    public async Task<CreditResponseDto> UpdateCreditHistoryAsync(Guid customerId, UpdateCreditDto dto)
    {
        var credit = await _context.CreditHistories
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (credit == null)
        {
            credit = new CreditHistory
            {
                CustomerId = customerId,
                CreditScore = dto.CreditScore,
                ReportDate = DateTime.UtcNow,
                TotalDebt = dto.TotalDebt ?? 0,
                AvailableCredit = dto.AvailableCredit ?? 0,
                NumberOfAccounts = dto.NumberOfAccounts ?? 0,
                LatePayments = dto.LatePayments ?? 0
            };
            _context.CreditHistories.Add(credit);
        }
        else
        {
            credit.CreditScore = dto.CreditScore;
            credit.ReportDate = DateTime.UtcNow;
            if (dto.TotalDebt.HasValue) credit.TotalDebt = dto.TotalDebt.Value;
            if (dto.AvailableCredit.HasValue) credit.AvailableCredit = dto.AvailableCredit.Value;
            if (dto.NumberOfAccounts.HasValue) credit.NumberOfAccounts = dto.NumberOfAccounts.Value;
            if (dto.LatePayments.HasValue) credit.LatePayments = dto.LatePayments.Value;
            credit.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return MapCreditToDto(credit);
    }

    #region Mapping Methods

    private static CustomerResponseDto MapToResponseDto(Models.Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            DateOfBirth = customer.DateOfBirth,
            Age = customer.Age,
            Address = customer.Address == null ? null : new AddressResponseDto
            {
                Id = customer.Address.Id,
                Street = customer.Address.Street,
                Unit = customer.Address.Unit,
                City = customer.Address.City,
                State = customer.Address.State,
                ZipCode = customer.Address.ZipCode,
                Country = customer.Address.Country
            },
            Employments = customer.Employments.Select(MapEmploymentToDto).ToList(),
            CreditHistory = customer.CreditHistory == null ? null : MapCreditToDto(customer.CreditHistory),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    private static EmploymentResponseDto MapEmploymentToDto(Employment emp)
    {
        return new EmploymentResponseDto
        {
            Id = emp.Id,
            EmployerName = emp.EmployerName,
            JobTitle = emp.JobTitle,
            EmploymentType = emp.EmploymentType.ToString(),
            AnnualIncome = emp.AnnualIncome,
            StartDate = emp.StartDate,
            EndDate = emp.EndDate,
            IsCurrent = emp.IsCurrent,
            YearsEmployed = emp.YearsEmployed
        };
    }

    private static CreditResponseDto MapCreditToDto(CreditHistory credit)
    {
        return new CreditResponseDto
        {
            Id = credit.Id,
            CreditScore = credit.CreditScore,
            CreditRating = credit.CreditRating,
            ReportDate = credit.ReportDate,
            CreditBureau = credit.CreditBureau,
            TotalDebt = credit.TotalDebt,
            AvailableCredit = credit.AvailableCredit,
            DebtToIncomeRatio = credit.DebtToIncomeRatio,
            NumberOfAccounts = credit.NumberOfAccounts,
            LatePayments = credit.LatePayments
        };
    }

    #endregion
}
