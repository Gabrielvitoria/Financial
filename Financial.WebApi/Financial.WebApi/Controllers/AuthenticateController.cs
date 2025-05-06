using Financial.Common.Dto;
using Financial.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Financial.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthenticateController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginUserDto login)
        {
            try
            {
                var token = _tokenService.GenerateToken(login.UserName, login.Password);

                return Ok(new
                {
                    user = token.Item1,
                    token = token.Item2
                });
            }
            catch (ApplicationException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();

            }
        }

    }
}
