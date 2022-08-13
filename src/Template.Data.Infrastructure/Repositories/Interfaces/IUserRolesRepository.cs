using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Repositories.Interfaces;

public interface IUserRolesRepository : IRepository<UserRoles, NpgsqlConnection>
{
    IEnumerable<UserRoles> GetRoleIdsByUserId(object id);

    IEnumerable<UserRoles> GetAllByRoleId(object id);
}