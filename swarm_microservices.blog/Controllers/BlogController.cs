using Microsoft.AspNetCore.Mvc;

namespace swarm_microservices.auth.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    [HttpGet("GetBlogs")]
    public IActionResult GetBlogs()
    {
        return Ok("Blog API");
    }
}
