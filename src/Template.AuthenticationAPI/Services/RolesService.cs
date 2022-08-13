using Npgsql;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.AuthenticationAPI.Services;

public class RolesService : IRolesService
{
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly IUsersRepository _usersRepo;
    private readonly IRolesRepository _rolesRepo;
    private readonly IUserRolesRepository _usersRolesRepo;

    public RolesService(IUnitOfWork<NpgsqlConnection> unitOfWork,
        IUsersRepository usersRepo,
        IRolesRepository rolesRepo,
        IUserRolesRepository usersRolesRepo)
    {
        _unitOfWork = unitOfWork;
        _usersRepo = usersRepo;
        _rolesRepo = rolesRepo;
        _usersRolesRepo = usersRolesRepo;
        
        _usersRepo.Attach(_unitOfWork);
        _rolesRepo.Attach(_unitOfWork);
        _usersRolesRepo.Attach(_unitOfWork);
    }
    
    public List<Role> FindUserRolesAsync(int userId)
    {
        List<Role> roles = new();
        try
        {
            _unitOfWork.Begin();
            var rolesForUser = _usersRolesRepo.GetRoleIdsByUserId(userId);
            roles.AddRange(rolesForUser.Select(role => _rolesRepo.Query(role.RoleId)));
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }
        return roles;
    }

    public bool IsUserInRoleAsync(int userId, string roleName)
    {
        try
        {
            _unitOfWork.Begin();
            var rolesForUser = _usersRolesRepo.GetRoleIdsByUserId(userId);
            var roles = rolesForUser.Select(r => _rolesRepo.Query(r.RoleId)).ToList();
            _unitOfWork.Commit();
            return roles.Any(r => r.RoleName == roleName);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }
    }

    public List<User> FindUsersInRoleAsync(string roleName)
    {
        List<User> users = new();
        try
        {
            _unitOfWork.Begin();
            var role = _rolesRepo.GetRoleByName(roleName);
            var userRoles = _usersRolesRepo.GetAllByRoleId(role.Id);
            users.AddRange(userRoles.Select(v => _usersRepo.Query(v.UserId)));
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }
        return users;
    }
}