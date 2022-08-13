using Microsoft.Extensions.Options;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Repositories.Integration;

public class RolesRepository : 
    Repository<Roles>, IRolesRepository
{
    public RolesRepository(IOptions<DbSettings> options)
        : base(options)
    {
    }

    public Roles GetRoleByName(string roleName) => 
        Query(x => x.RoleName == roleName,
            transaction: _unitOfWork?.Transaction)?.FirstOrDefault();
}