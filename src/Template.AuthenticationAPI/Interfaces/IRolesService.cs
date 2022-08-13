using Template.Data.Infrastructure.Entities;

namespace Template.AuthenticationAPI.Interfaces;

public interface IRolesService
{
    List<Role> FindUserRolesAsync(int userId);
    bool IsUserInRoleAsync(int userId, string roleName);
    List<User> FindUsersInRoleAsync(string roleName);
}