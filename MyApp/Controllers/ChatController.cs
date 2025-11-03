using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(IRealtimeService realtimeService) : ControllerBase
    {
        [HttpGet("rooms/{userId}")]
        public async Task<ActionResult> GetRoomsByUserId(int userId)
        {
            try
            {
                var rooms = await realtimeService.GetRoomsByUserIdAsync(userId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("messages/{roomId}")]
        public async Task<ActionResult<List<AppMessage>>> GetMessagesForRoom(int roomId)
        {
            try
            {
                var messages = await realtimeService.GetMessagesForRoomAsync(roomId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
