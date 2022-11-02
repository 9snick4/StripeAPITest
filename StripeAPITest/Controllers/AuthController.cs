using StripeAPITest.BusinessLayer.Extensions;
using StripeAPITest.BusinessLayer.Services;
using StripeAPITest.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StripeAPITest.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public AuthController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeAsync()
        {
            return Ok(await identityService.GetMeAsync(User.GetId()));
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok(await identityService.GetUsersAsync());
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await identityService.LoginAsync(request,false);
            if (response != null)
                return Ok(response);

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var response = await identityService.RefreshTokenAsync(request);
            if (response != null)
                return Ok(response);

            return BadRequest();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await identityService.RegisterAsync(request);

            return StatusCode(response.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
        }

        [HttpPost("addUser")]
        [ProducesResponseType(typeof(NewUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsync(NewUser item)
        {
            return Ok(await identityService.InsertUserAsync(item));
        }


    }
}
