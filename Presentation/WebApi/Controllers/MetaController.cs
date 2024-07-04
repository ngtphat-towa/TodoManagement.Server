using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class MetaController : BaseApiController
{
    [HttpGet]
    public ActionResult Result()
    {
        return Ok("This is meta controller");
    }
}
