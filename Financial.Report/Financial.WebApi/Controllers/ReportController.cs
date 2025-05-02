using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financial.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "gerente")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Relatorio de vendas");
        }
        
    }
}
