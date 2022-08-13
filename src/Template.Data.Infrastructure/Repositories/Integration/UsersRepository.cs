using Microsoft.Extensions.Options;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Repositories.Integration;

public class UsersRepository : 
    Repository<User>, IUsersRepository
{
    public UsersRepository(IOptions<DbSettings> options)
        : base(options)
    {
    }
    
    public User FindByUsernameAndPassword(string username, string password) => 
        Query(x => x.Username == username && x.Password == password,
            transaction: _unitOfWork?.Transaction)?.FirstOrDefault();
}