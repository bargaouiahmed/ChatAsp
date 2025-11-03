


using System.ComponentModel.DataAnnotations;
using MyApp.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    public List<AppUserConnection> Connections { get; set; } = new();
    public List<AppMessage> SentMessages { get; set; } = new();
}