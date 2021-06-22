using Microsoft.AspNetCore.Mvc;

namespace Calypso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("I am ok");
        }
    }
}
