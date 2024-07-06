using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MetaController : ControllerBase
{
    [HttpGet]
    public ActionResult Result()
    {
        return Ok("This is meta controller");
    }
}
