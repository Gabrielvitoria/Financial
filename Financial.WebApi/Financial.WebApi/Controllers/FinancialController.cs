using Financial.Common.Dto;
using Financial.Domain.Dtos;
using Financial.Service;
using Financial.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financial.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "gerente")]
    [ApiController]
    public class FinancialController : ControllerBase
    {

        private readonly IProcessLaunchservice _processLaunchservice;

        public FinancialController(IProcessLaunchservice processLaunchservice)
        {
            _processLaunchservice = processLaunchservice;
        }


        [HttpPost]
        [Route("Launch")]
        public async Task<IActionResult> NewFinancialLaunchAsync([FromBody] CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            try
            {
                return Ok( await _processLaunchservice.ProcessNewLaunchAsync(createFinanciallaunchDto));
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

        [HttpPost]
        [Route("Pay")]
        public async Task<IActionResult> NewPayOfLaunchAsync([FromBody] PayFinanciallaunchDto payFinanciallaunchDto)
        {
            try
            {
                return Ok(await _processLaunchservice.ProcessPayLaunchAsync(payFinanciallaunchDto));
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
