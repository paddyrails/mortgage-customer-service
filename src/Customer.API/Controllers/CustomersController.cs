using Microsoft.AspNetCore.Mvc;
using Customer.API.DTOs;
using Customer.API.Services;

namespace Customer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponseDto>>>> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(ApiResponse<IEnumerable<CustomerResponseDto>>.SuccessResponse(customers));
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> GetById(Guid id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        
        if (customer == null)
            return NotFound(ApiResponse<CustomerResponseDto>.FailResponse($"Customer {id} not found"));

        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer));
    }

    /// <summary>
    /// Get customer by email
    /// </summary>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> GetByEmail(string email)
    {
        var customer = await _customerService.GetCustomerByEmailAsync(email);
        
        if (customer == null)
            return NotFound(ApiResponse<CustomerResponseDto>.FailResponse($"Customer with email {email} not found"));

        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer));
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<CustomerResponseDto>.FailResponse("Validation failed", errors));
        }

        try
        {
            var customer = await _customerService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id },
                ApiResponse<CustomerResponseDto>.SuccessResponse(customer, "Customer created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return BadRequest(ApiResponse<CustomerResponseDto>.FailResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update a customer
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var customer = await _customerService.UpdateCustomerAsync(id, dto);
        
        if (customer == null)
            return NotFound(ApiResponse<CustomerResponseDto>.FailResponse($"Customer {id} not found"));

        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer, "Customer updated successfully"));
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        
        if (!result)
            return NotFound(ApiResponse<object>.FailResponse($"Customer {id} not found"));

        return Ok(ApiResponse<object>.SuccessResponse(new { Id = id }, "Customer deleted successfully"));
    }

    /// <summary>
    /// Get customer's credit history
    /// </summary>
    [HttpGet("{id:guid}/credit")]
    [ProducesResponseType(typeof(ApiResponse<CreditResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse<CreditResponseDto>>> GetCreditHistory(Guid id)
    {
        var credit = await _customerService.GetCreditHistoryAsync(id);
        
        if (credit == null)
            return NotFound(ApiResponse<CreditResponseDto>.FailResponse($"Credit history for customer {id} not found"));

        return Ok(ApiResponse<CreditResponseDto>.SuccessResponse(credit));
    }

    /// <summary>
    /// Update customer's credit history
    /// </summary>
    [HttpPut("{id:guid}/credit")]
    [ProducesResponseType(typeof(ApiResponse<CreditResponseDto>), 200)]
    public async Task<ActionResult<ApiResponse<CreditResponseDto>>> UpdateCreditHistory(Guid id, [FromBody] UpdateCreditDto dto)
    {
        var credit = await _customerService.UpdateCreditHistoryAsync(id, dto);
        return Ok(ApiResponse<CreditResponseDto>.SuccessResponse(credit, "Credit history updated"));
    }

    /// <summary>
    /// Get customer's employments
    /// </summary>
    [HttpGet("{id:guid}/employments")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmploymentResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmploymentResponseDto>>>> GetEmployments(Guid id)
    {
        var employments = await _customerService.GetEmploymentsAsync(id);
        return Ok(ApiResponse<IEnumerable<EmploymentResponseDto>>.SuccessResponse(employments));
    }

    /// <summary>
    /// Add employment to customer
    /// </summary>
    [HttpPost("{id:guid}/employments")]
    [ProducesResponseType(typeof(ApiResponse<EmploymentResponseDto>), 201)]
    public async Task<ActionResult<ApiResponse<EmploymentResponseDto>>> AddEmployment(Guid id, [FromBody] CreateEmploymentDto dto)
    {
        var employment = await _customerService.AddEmploymentAsync(id, dto);
        return CreatedAtAction(nameof(GetEmployments), new { id },
            ApiResponse<EmploymentResponseDto>.SuccessResponse(employment, "Employment added"));
    }
}
