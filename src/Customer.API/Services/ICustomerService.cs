using Customer.API.DTOs;

namespace Customer.API.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id);
    Task<CustomerResponseDto?> GetCustomerByEmailAsync(string email);
    Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto);
    Task<bool> DeleteCustomerAsync(Guid id);
    
    // Employment
    Task<EmploymentResponseDto> AddEmploymentAsync(Guid customerId, CreateEmploymentDto dto);
    Task<IEnumerable<EmploymentResponseDto>> GetEmploymentsAsync(Guid customerId);
    
    // Credit
    Task<CreditResponseDto?> GetCreditHistoryAsync(Guid customerId);
    Task<CreditResponseDto> UpdateCreditHistoryAsync(Guid customerId, UpdateCreditDto dto);
}
