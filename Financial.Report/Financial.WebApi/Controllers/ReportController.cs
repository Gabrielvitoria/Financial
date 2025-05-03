using Financial.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financial.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "gerente")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IFinanciallaunchService _financiallaunchService;
        public ReportController(IFinanciallaunchService financiallaunchService)
        {
            _financiallaunchService = financiallaunchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                return Ok(await _financiallaunchService.GetSaldoDiarioAsync());
            }
            catch (ApplicationException aex)
            {
                return NotFound(aex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
