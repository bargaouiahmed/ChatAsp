using System;

namespace MyApp.Models;

public class AppUserConnection
{
    public int Id { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DisconnectedAt { get; set; }

    // Navigation property
    public AppUser? User { get; set; }
}
