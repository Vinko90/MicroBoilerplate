using Template.Data.Infrastructure.Entities;

namespace Template.AuthenticationAPI.Interfaces;

public interface IUsersService
{
    Task<string> GetSerialNumberAsync(int userId);
    Task<User> FindUserAsync(string username, string password);
    ValueTask<User> FindUserAsync(int userId);
    Task UpdateUserLastActivityDateAsync(int userId);
    ValueTask<User> GetCurrentUserAsync();
    int GetCurrentUserId();
    Task<(bool Succeeded, string Error)> ChangePasswordAsync(User user, string currentPassword, string newPassword);
}