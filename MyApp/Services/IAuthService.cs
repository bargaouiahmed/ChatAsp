using System;
using MyApp.Services.DTOS;

namespace MyApp.Services;

public interface IAuthService
{
    public Task<AppUser?> Register(RegisterReq req);
    public Task<TokenPairResponse> Login(LoginReq req);

    public Task<string> RefreshToken(string token);
    public Task Logout(int userId);
}
