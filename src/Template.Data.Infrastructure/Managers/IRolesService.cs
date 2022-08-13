using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Managers;

public interface IRolesService
{
    List<Roles> FindUserRolesAsync(int userId);
    bool IsUserInRoleAsync(int userId, string roleName);
    List<Users> FindUsersInRoleAsync(string roleName);
}