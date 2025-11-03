using System;

namespace MyApp.Models;

public class AppMessage
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AppRoom? Room { get; set; }
    public AppUser? Sender { get; set; }
}
