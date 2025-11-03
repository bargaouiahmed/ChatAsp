using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models;

public class AppRoom
{
    public int Id { get; set; }
    
    [ForeignKey("User1")]
    public int UserId1 { get; set; }
    
    [ForeignKey("User2")]
    public int UserId2 { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }

    // Navigation properties
    public AppUser? User1 { get; set; }
    public AppUser? User2 { get; set; }
    public List<AppMessage> Messages { get; set; } = new();
}
