using System;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using MyApp.Services.DTOS;

namespace MyApp.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<List<UserRepresentation>> GetAllUsers(int currentUserId)
    {
        var users = await context.Users
            .Where(u => u.Id != currentUserId)
            .Select(u => new UserRepresentation
            {
                Id = u.Id,
                Username = u.Username
            })
            .OrderBy(u => u.Username)
            .ToListAsync();

        return users;
    }
}
