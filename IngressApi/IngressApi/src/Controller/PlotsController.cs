using Microsoft.AspNetCore.Mvc;

namespace IngressApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlotsController : ControllerBase
{
    [HttpGet("viewDataPlots")]
    public IActionResult ViewDataPlots()
    {
        return Ok("certo");
    }
    
}