using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Repositories.Interfaces;

public interface IUserTokensRepository : IRepository<UserToken, NpgsqlConnection>
{
    int DeleteExpiredRefreshTokens();

    int DeleteTokensWithSameRefreshTokenSource(string source);

    int DeleteTokenForUserId(int userId);
    
    UserToken GetTokenByRefreshTokenIdHash(string refreshTokenIdHash);

    UserToken GetTokenByHashAndUserId(string accessToken, int userId);
}