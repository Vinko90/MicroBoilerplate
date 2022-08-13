using Microsoft.Extensions.Options;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Repositories.Integration;

public class UserTokensRepository : 
    Repository<UserTokens>, IUserTokensRepository
{
    public UserTokensRepository(IOptions<DbSettings> options) 
        : base(options)
    {
    }
}