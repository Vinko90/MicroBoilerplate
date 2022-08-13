using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Repositories.Interfaces;

public interface IUserRolesRepository : IRepository<UserRole, NpgsqlConnection>
{
    IEnumerable<UserRole> GetRoleIdsByUserId(object id);

    IEnumerable<UserRole> GetAllByRoleId(object id);
}