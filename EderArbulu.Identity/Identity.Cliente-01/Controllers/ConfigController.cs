using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Cliente_01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigController : ControllerBase
    {
        [HttpGet("Accesos")]
        public IEnumerable<string> Accesos()
        {
            return Enumerable.Range(1, 5).Select(i=>"Accesos " + i)
                .ToArray();
        }
    }
}
