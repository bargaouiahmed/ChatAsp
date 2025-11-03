using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyApp.Hubs;
using MyApp.Models;

namespace MyApp.Services;

public class RealtimeService(AppDbContext context, IHubContext<ChatHub> chatHubContext):IRealtimeService
{
    public async Task<AppUserConnection> StoreConnectionAsync(int userId, string connectionId)

    {
        if (await context.UserConnections.AnyAsync(c => c.ConnectionId == connectionId && c.DisconnectedAt == null && c.UserId == userId))
        
        {
            throw new Exception("Connection ID already exists");
        }
        AppUserConnection connection = new();
        connection.UserId = userId;
        connection.ConnectionId = connectionId;
        connection.ConnectedAt = DateTime.UtcNow; 
        context.UserConnections.Add(connection);
        await context.SaveChangesAsync();
        return connection;
        
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        await context.UserConnections
            .Where(c => c.ConnectionId == connectionId)
            .ForEachAsync(c =>
            {
                c.DisconnectedAt = DateTime.UtcNow;
            });
        await context.SaveChangesAsync();

    }

    public async Task<AppRoom?> CreateOrGetRoomAsync(int userId1, int userId2)
    {
        
        var room =await  context.Rooms
            .FirstOrDefaultAsync(r =>
                (r.UserId1 == userId1 && r.UserId2 == userId2) ||
                (r.UserId1 == userId2 && r.UserId2 == userId1)
            );

        if (room != null)
        {
            return room;
        }

        AppRoom newRoom = new()
        {
            UserId1 = userId1,
            UserId2 = userId2,
            CreatedAt = DateTime.UtcNow
        };

        context.Rooms.Add(newRoom);
        await context.SaveChangesAsync();
        await chatHubContext.Clients.Users(new List<string> {userId1.ToString(), userId2.ToString()}).SendAsync("RoomCreated", new RoomResponse
        {
            Id = newRoom.Id,
            UserId1 = newRoom.UserId1,
            UserId2 = newRoom.UserId2,
            User1Connected = (await context.Users.FindAsync(userId1))?.Connections.Any(c => c.DisconnectedAt == null) ?? false,
            User2Connected = (await context.Users.FindAsync(userId2))?.Connections.Any(c => c.DisconnectedAt == null) ?? false,
            CreatedAt = newRoom.CreatedAt,
            LastMessageAt = newRoom.LastMessageAt,
            User1Username = (await context.Users.FindAsync(userId1))?.Username ?? string.Empty,
            User2Username = (await context.Users.FindAsync(userId2))?.Username ?? string.Empty
        });


        return newRoom;
    }

    public async Task<AppMessage> SaveMessageAsync(int roomId, int senderId, string content)
    {
        var message = new AppMessage
        {
            RoomId = roomId,
            SenderId = senderId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync();

        return message;
    }
    public async Task<AppMessage> StoreMessageAsync(int roomId, int senderId, string content)
    {
        var message = new AppMessage
        {
            RoomId = roomId,
            SenderId = senderId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };
        context.Messages.Add(message);
        await context.SaveChangesAsync();
        return message;
    }

    public async Task<List<AppMessage>> GetMessagesForRoomAsync(int roomId)
    {
        return await context.Messages
            .Where(m => m.RoomId == roomId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<List<AppUser?>> GetConnectedPartnersInRoomsAsync(int userId)
    {
        var rooms = await context.Rooms
            .Where(r => r.UserId1 == userId || r.UserId2 == userId)
            .Include(r => r.User1)
                .ThenInclude(u => u.Connections.Where(c => c.DisconnectedAt == null))
            .Include(r => r.User2)
                .ThenInclude(u => u.Connections.Where(c => c.DisconnectedAt == null))
            .ToListAsync();

        List<AppUser?> partners = [];
        foreach (var room in rooms)
        {
            AppUser? partner = null;
            
            if (room.UserId1 == userId)
            {
                partner = room.User2;
            }
            else
            {
                partner = room.User1;
            }

            if (partner?.Connections?.Count != 0 && partner?.Connections != null)
            {
                partners.Add(partner);
            }
        }

        return partners;
    }
    public async Task<List<RoomResponse>> GetRoomsByUserIdAsync(int userId)
    {
        return await context.Rooms
            .Where(r => r.UserId1 == userId || r.UserId2 == userId)
            .Select(r => new RoomResponse
            {
                Id = r.Id,
                UserId1 = r.UserId1,
                UserId2 = r.UserId2,
                User1Connected = r.User1 != null && r.User1.Connections.Any(c => c.DisconnectedAt == null),
                User2Connected = r.User2 != null && r.User2.Connections.Any(c => c.DisconnectedAt == null),
                CreatedAt = r.CreatedAt,
                LastMessageAt = r.LastMessageAt,
                User1Username = r.User1 != null ? r.User1.Username : string.Empty,
                User2Username = r.User2 != null ? r.User2.Username : string.Empty

            })
            .ToListAsync();
    }
    public async Task RemoveConnectionAsync(int userId, string connectionId)
    {
        var connection = await context.UserConnections
            .FirstOrDefaultAsync(c => c.ConnectionId == connectionId && c.UserId == userId && c.DisconnectedAt == null);

        if (connection != null)
        {
            connection.DisconnectedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
    // public async Task<AppUser> GetUserByUsernameAsync(string username)
    // {
    //     var user = await context.Users
    //         .FirstOrDefaultAsync(u => u.Username == username);

    //     if (user == null)
    //     {
    //         throw new InvalidOperationException("User not found.");
    //     }

    //     return user;
    // }
    
}
