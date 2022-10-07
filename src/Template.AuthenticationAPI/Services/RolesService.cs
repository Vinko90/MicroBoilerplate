using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.AuthenticationAPI.Services;

public class RolesService : IRolesService
{
    private readonly IUsersRepository _usersRepo;
    private readonly IRolesRepository _rolesRepo;
    private readonly IUserRolesRepository _usersRolesRepo;

    public RolesService(
        IUsersRepository usersRepo,
        IRolesRepository rolesRepo,
        IUserRolesRepository usersRolesRepo)
    {
        _usersRepo = usersRepo;
        _rolesRepo = rolesRepo;
        _usersRolesRepo = usersRolesRepo;
    }
    
    public List<Role> FindUserRolesAsync(int userId)
    {
        List<Role> roles = new();
        var rolesForUser = _usersRolesRepo.GetRoleIdsByUserId(userId);
        roles.AddRange(rolesForUser.Select(role => _rolesRepo.Query(role.RoleId)));
        return roles;
    }

    public bool IsUserInRoleAsync(int userId, string roleName)
    {
        var rolesForUser = _usersRolesRepo.GetRoleIdsByUserId(userId);
        var roles = rolesForUser.Select(r => _rolesRepo.Query(r.RoleId)).ToList();
        return roles.Any(r => r.RoleName == roleName);
    }

    public List<User> FindUsersInRoleAsync(string roleName)
    {
        List<User> users = new();
        var role = _rolesRepo.GetRoleByName(roleName);
        var userRoles = _usersRolesRepo.GetAllByRoleId(role.Id);
        users.AddRange(userRoles.Select(v => _usersRepo.Query(v.UserId)));
        return users;
    }
}