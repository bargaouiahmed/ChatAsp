using System;
using MyApp.Hubs;
using MyApp.Models;

namespace MyApp.Services;

public interface IRealtimeService
{
    public Task<AppUserConnection> StoreConnectionAsync(int userId, string connectionId);
    public Task RemoveConnectionAsync(string connectionId);
    public Task<AppRoom?> CreateOrGetRoomAsync(int userId1, int userId2);
    public Task<AppMessage> SaveMessageAsync(int roomId, int senderId, string content);
    public Task<List<AppMessage>> GetMessagesForRoomAsync(int roomId);
    public Task<AppMessage> StoreMessageAsync(int roomId, int senderId, string content);
    public Task<List<RoomResponse>> GetRoomsByUserIdAsync(int userId);
    public Task<List<AppUser?>> GetConnectedPartnersInRoomsAsync(int userId);
    // public  Task<AppUser> GetUserByUsernameAsync(string username);


}