using Context;
using Microsoft.AspNetCore.Mvc;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController(EfContext context) : ControllerBase
{
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var canConnect = await context.Database.CanConnectAsync();
        return canConnect ? Ok(new { status = "Healthy" }) : StatusCode(500, "Database connection failed.");
    }
}