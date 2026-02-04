using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IngressApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class HealthController(IHostEnvironment env) : ControllerBase
{
    [HttpGet("status")]
    [AllowAnonymous]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            service = env.ApplicationName,
            environment = env.EnvironmentName,
            timestamp = DateTimeOffset.UtcNow,
            status = "Healthy"
        });
    }
}