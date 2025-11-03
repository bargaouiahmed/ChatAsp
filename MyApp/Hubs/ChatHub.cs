using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Hubs;

public class ChatHub(IRealtimeService realtimeService) : Hub
{

    private int GetCurrentUserId()
    {
        var userIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        else
        {
            throw new Exception("Invalid user ID in claims.");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            int userId = GetCurrentUserId();
            await realtimeService.StoreConnectionAsync(userId, Context.ConnectionId);

            var partners = await realtimeService.GetConnectedPartnersInRoomsAsync(userId);
            var partnerIds = partners
                .Where(p => p?.Id != null)
                .Select(p => p!.Id.ToString())
                .ToList();

            if (partnerIds.Count > 0)
            {
                // Notify partners that this user is now online
                await Clients.Users(partnerIds).SendAsync("UserStatusChanged", userId, true);
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnConnectedAsync error: {ex.Message}");
            await base.OnConnectedAsync();
        }
    }


    public async Task<AppRoom?> CreateOrJoinChatRoom(int otherUserId)
    {
        try
        {
            int currentUserId = GetCurrentUserId();
            var room = await realtimeService.CreateOrGetRoomAsync(currentUserId, otherUserId) ?? throw new InvalidOperationException("Failed to create or get room.");
            if (room != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{room.Id.ToString()}");
                await Clients.Group($"room-{room.Id.ToString()}")
                    .SendAsync("UserJoined", currentUserId, room.Id);
                return room;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateOrJoinChatRoom error: {ex.Message}");
            throw;
        }
    }

    public async Task SendMessage(int roomId, string messageContent)
    {
        try
        {
            int currentUserId = GetCurrentUserId();

            // Save message to database
            await realtimeService.SaveMessageAsync(roomId, currentUserId, messageContent);

            // Broadcast to room group
            await Clients.Group($"room-{roomId}")
                .SendAsync("ReceiveMessage", new { userId = currentUserId, message = messageContent, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SendMessage error: {ex.Message}");
            throw;
        }
    }
    public async Task<List<AppMessage>> GetMessageHistory(int roomId)
    {
        try
        {
            List<AppMessage> messages = await realtimeService.GetMessagesForRoomAsync(roomId);
            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMessageHistory error: {ex.Message}");
            throw;
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            int userId = GetCurrentUserId();
            await realtimeService.RemoveConnectionAsync(Context.ConnectionId);

            var partners = await realtimeService.GetConnectedPartnersInRoomsAsync(userId);
            var partnerIds = partners
                .Where(p => p?.Id != null)
                .Select(p => p!.Id.ToString())
                .ToList();

            if (partnerIds.Count > 0)
            {
                // Notify partners that this user is now offline
                await Clients.Users(partnerIds).SendAsync("UserStatusChanged", userId, false);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnDisconnectedAsync error: {ex.Message}");
            await base.OnDisconnectedAsync(exception);

        }
    }


}