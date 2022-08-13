using Template.AuthenticationAPI.Common;
using Template.Data.Infrastructure.Entities;

namespace Template.AuthenticationAPI.Interfaces;

public interface ITokenFactoryService
{
    Task<JwtTokensData> CreateJwtTokensAsync(User user);
    string GetRefreshTokenSerial(string refreshTokenValue);
}