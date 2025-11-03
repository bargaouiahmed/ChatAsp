using System;

namespace MyApp.Services.DTOS;

public class TokenPairResponse

{
    public int UserId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
