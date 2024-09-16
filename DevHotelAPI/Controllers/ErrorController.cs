using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevHotelAPI.Controllers
{
    [Route("api/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError() => Problem();

    }
}
