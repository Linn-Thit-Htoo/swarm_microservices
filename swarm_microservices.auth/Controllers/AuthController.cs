using Microsoft.AspNetCore.Mvc;

namespace swarm_microservices.auth.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("GetUserInfo")]
    public IActionResult GetUserInfo()
    {
        return Ok("Auth API");
    }
}
