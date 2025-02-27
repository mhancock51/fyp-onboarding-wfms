using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;

        public AuthController(IAuthLogic authLogic)
        {
            _authLogic = authLogic;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string emailAddress, string hashedPassword)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return BadRequest();
            }

            var result = await _authLogic.LoginUser(emailAddress, hashedPassword);
            return StatusCode(result.HttpCode, result);
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestToken()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            foreach (var claim in claimsIdentity.Claims)
            {
                Console.WriteLine(claim.Type + ":" + claim.Value);
            }
            return Ok();
        }
    }
}
