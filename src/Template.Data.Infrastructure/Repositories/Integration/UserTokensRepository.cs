using Microsoft.Extensions.Options;
using Template.Data.Infrastructure.Common;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Repositories.Integration;

public class UserTokensRepository : 
    Repository<UserToken>, IUserTokensRepository
{
    public UserTokensRepository(IOptions<DbSettings> options) 
        : base(options)
    {
    }

    public int DeleteExpiredRefreshTokens() =>
        Delete(x => x.RefreshTokenExpiresDateTime < DateTime.Now.ToUniversalTime(),
            transaction: _unitOfWork.Transaction);

    public int DeleteTokensWithSameRefreshTokenSource(string source) =>
        Delete(t => t.RefreshTokenIdHashSource == source ||
                    t.RefreshTokenIdHash == source &&
                    t.RefreshTokenIdHashSource == null,
            transaction: _unitOfWork.Transaction);

    public int DeleteTokenForUserId(int userId) =>
        Delete(x => x.UserId == userId,
            transaction: _unitOfWork.Transaction);

    public UserToken GetTokenByRefreshTokenIdHash(string refreshTokenIdHash) => 
        Query(x => x.RefreshTokenIdHash == refreshTokenIdHash,
            transaction: _unitOfWork?.Transaction)?.FirstOrDefault();

    public UserToken GetTokenByHashAndUserId(string accessToken, int userId) => 
        Query(x => x.AccessTokenHash == accessToken && x.UserId == userId,
            transaction: _unitOfWork?.Transaction)?.FirstOrDefault();
}