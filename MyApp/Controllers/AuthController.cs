using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Services;
using MyApp.Services.DTOS;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] RegisterReq req)
        {
            try
            {
                var user = await authService.Register(req);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenPairResponse>> Login([FromBody] LoginReq req)
        {
            try
            {
                var token = await authService.Login(req);
                return Ok(token);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("auth-status")]
        public ActionResult<bool> Protected()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Ok(false);
            }
            return Ok(true);
        }


        [HttpGet("health")]
        public ActionResult<string> HealthCheck()
        {
            return Ok("Auth service is healthy");
        }


        [HttpGet("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromQuery] string token)
        {
            try
            {
                Console.WriteLine(token);
                token = token.Replace(" ", "+");
                var newAccessToken = await authService.RefreshToken(token);
                return Ok(newAccessToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }







    }   

}
