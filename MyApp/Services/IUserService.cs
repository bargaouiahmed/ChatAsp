using System;
using MyApp.Services.DTOS;

namespace MyApp.Services;

public interface IUserService
{
    public Task<List<UserRepresentation>> GetAllUsers(int currentUserId);
}
