using System.Globalization;
using Npgsql;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;
using Template.Shared.Models;

namespace Template.AuthenticationAPI.Services;

public class TokenStoreService : ITokenStoreService
{
    private readonly ISecurityService _securityService;
    private readonly ITokenFactoryService _tokenFactoryService;
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly IUserTokensRepository _tokenRepo;

    public TokenStoreService(
        IUnitOfWork<NpgsqlConnection> unitOfWork,
        IUserTokensRepository tokenRepo,
        ISecurityService securityService,
        ITokenFactoryService tokenFactoryService)
    {
        _unitOfWork = unitOfWork;
        _tokenRepo = tokenRepo;
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _tokenFactoryService = tokenFactoryService ?? throw new ArgumentNullException(nameof(tokenFactoryService));
        
        _tokenRepo.Attach(_unitOfWork);
    }

    public async Task AddUserTokenAsync(UserToken userToken)
    {
        if (userToken == null)
        {
            throw new ArgumentNullException(nameof(userToken));
        }

        if (!BearerTokensOptions.AllowMultipleLoginsFromTheSameUser)
        {
            await InvalidateUserTokensAsync(userToken.UserId);
        }

        await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);

        try
        {
            _unitOfWork.Begin();
            _tokenRepo.Add<int>(userToken);
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken,
        string refreshTokenSourceSerial)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var now = DateTime.UtcNow;
        var token = new UserToken
        {
            UserId = user.Id,
            // Refresh token handles should be treated as secrets and should be stored hashed
            RefreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial),
            RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSourceSerial)
                ? null
                : _securityService.GetSha256Hash(refreshTokenSourceSerial),
            AccessTokenHash = _securityService.GetSha256Hash(accessToken),
            RefreshTokenExpiresDateTime = now.AddMinutes(BearerTokensOptions.RefreshTokenExpirationMinutes),
            AccessTokenExpiresDateTime = now.AddMinutes(BearerTokensOptions.AccessTokenExpirationMinutes)
        };
        await AddUserTokenAsync(token);
    }

    public Task DeleteExpiredTokensAsync()
    {
        try
        {
            _unitOfWork.Begin();
            _tokenRepo.DeleteExpiredRefreshTokens();
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task DeleteTokenAsync(string refreshTokenValue)
    {
        var token = await FindTokenAsync(refreshTokenValue);
        if (token != null)
        {
            try
            {
                _unitOfWork.Begin();
                _tokenRepo.Delete(token);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
        {
            return Task.CompletedTask;
        }

        try
        {
            _unitOfWork.Begin();
            _tokenRepo.DeleteTokensWithSameRefreshTokenSource(refreshTokenIdHashSource);
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue)
    {
        if (!string.IsNullOrWhiteSpace(userIdValue) &&
            int.TryParse(userIdValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var userId))
        {
            if (BearerTokensOptions.AllowSignoutAllUserActiveClients)
            {
                await InvalidateUserTokensAsync(userId);
            }
        }

        if (!string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
            if (!string.IsNullOrWhiteSpace(refreshTokenSerial))
            {
                var refreshTokenIdHashSource = _securityService.GetSha256Hash(refreshTokenSerial);
                await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
            }
        }

        await DeleteExpiredTokensAsync();
    }

    public Task<UserToken> FindTokenAsync(string refreshTokenValue)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return Task.FromResult<UserToken>(null);
        }

        var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
        if (string.IsNullOrWhiteSpace(refreshTokenSerial))
        {
            return Task.FromResult<UserToken>(null);
        }

        var refreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial);
        var token = _tokenRepo.GetTokenByRefreshTokenIdHash(refreshTokenIdHash);
        return Task.FromResult(token);
    }

    public Task InvalidateUserTokensAsync(int userId)
    {
        try
        {
            _unitOfWork.Begin();
            _tokenRepo.DeleteTokenForUserId(userId);
            _unitOfWork.Commit();

        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsValidTokenAsync(string accessToken, int userId)
    {
        var accessTokenHash = _securityService.GetSha256Hash(accessToken);
        var token = _tokenRepo.GetTokenByHashAndUserId(accessTokenHash, userId);
        return Task.FromResult(token?.AccessTokenExpiresDateTime >= DateTime.UtcNow);
    }
}