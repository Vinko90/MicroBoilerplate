using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Repositories.Interfaces;

public interface IRolesRepository : IRepository<Role, NpgsqlConnection>
{
    Role GetRoleByName(string roleName);
}