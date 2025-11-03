using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Services;
using MyApp.Services.DTOS;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [Authorize]
        [HttpGet("users")]
        public async Task<ActionResult<List<UserRepresentation>>> GetAllUsers()
        {
            try
            {
                int loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserId = loggedInUserId;
                var users = await userService.GetAllUsers(currentUserId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
