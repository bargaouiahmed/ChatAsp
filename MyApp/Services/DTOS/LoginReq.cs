using System;

namespace MyApp.Services.DTOS;

public class LoginReq
{
    public string? Username { get; set; }
    public string Password { get; set; } = string.Empty;

}
