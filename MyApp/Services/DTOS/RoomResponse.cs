using System;

namespace MyApp.Hubs;

public class RoomResponse
{
    public int Id { get; set; }
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
    public bool User1Connected { get; set; }
    public bool User2Connected { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? User1Username { get; set; } 
    public string? User2Username { get; set; } 

}
