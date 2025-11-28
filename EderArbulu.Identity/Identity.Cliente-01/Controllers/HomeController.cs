using Microsoft.AspNetCore.Mvc;

namespace Identity.Cliente_01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Bienvenido al Cliente 1";
        }
    }
}
