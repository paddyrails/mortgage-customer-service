using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "Customer.API",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }

    [HttpGet("live")]
    public IActionResult Live() => Ok(new { Status = "Alive" });

    [HttpGet("ready")]
    public IActionResult Ready() => Ok(new { Status = "Ready" });
}
