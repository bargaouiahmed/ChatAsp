using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.Hubs;
using MyApp.Models;
using MyApp.Services.DTOS;

namespace MyApp.Services;

public class AuthService(AppDbContext context, IConfiguration configuration, IHubContext<ChatHub> chatHubContext):IAuthService
{
    public async Task<AppUser?> Register(RegisterReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) ||
            string.IsNullOrWhiteSpace(req.Password))
        {
            throw new InvalidOperationException("Username, email, and password are required.");
        }
        
        if (await context.Users.AnyAsync(u => u.Username == req.Username))
        {
            throw new InvalidOperationException("User with the same username already exists.");
        }

        AppUser user = new();
        string hashedPassword = new PasswordHasher<AppUser>().HashPassword(user, req.Password);
        user.Username = req.Username;
        user.PasswordHash = hashedPassword;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        await chatHubContext.Clients.All.SendAsync("UserRegistered", new { user.Id, user.Username });
        return user;

    }
    
    private string CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(
issuer: configuration["AppSettings:Issuer"],
audience: configuration["AppSettings:Audience"],
claims: claims,
expires: DateTime.Now.AddMinutes(15),
signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            }
    public async Task<TokenPairResponse> Login(LoginReq req)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == req.Username) ?? throw new InvalidOperationException("Invalid username or password.");
        var verificationResult = new PasswordHasher<AppUser>().VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }
        return await CreateTokenPairResponse(user);

    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private async Task<TokenPairResponse> CreateTokenPairResponse(AppUser user)
    {
        string accessToken = CreateToken(user);
        string refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await context.SaveChangesAsync();
        return new TokenPairResponse
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };





    }
    
    public async Task<string> RefreshToken(string token)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == token) ?? throw new InvalidOperationException("Invalid refresh token.");
        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();
            throw new InvalidOperationException("Refresh token has expired.");
            
        }
        return CreateToken(user);
    }

public async Task Logout(int userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
        }
    }
}
