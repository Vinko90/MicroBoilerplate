using System.Globalization;
using System.Security.Claims;
using Npgsql;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.AuthenticationAPI.Services;

public class UsersService : IUsersService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ISecurityService _securityService;
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly IUsersRepository _usersRepo;

    public UsersService(
        IUnitOfWork<NpgsqlConnection> unitOfWork,
        IUsersRepository usersRepo,
        ISecurityService securityService,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _usersRepo = usersRepo;
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        
        _usersRepo.Attach(_unitOfWork);
    }

    public ValueTask<User> FindUserAsync(int userId)
    { 
        return new ValueTask<User>(_usersRepo.Query(userId));
    }

    public Task<User> FindUserAsync(string username, string password)
    {
        var passwordHash = _securityService.GetSha256Hash(password);
        var user = _usersRepo.FindByUsernameAndPassword(username, passwordHash);
        return Task.FromResult(user);
    }

    public async Task<string> GetSerialNumberAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        return user.SerialNumber;
    }

    public Task UpdateUserLastActivityDateAsync(int userId)
    {
        try
        {
            _unitOfWork.Begin();
            var user = _usersRepo.Query(userId);
            if (user.LastLoggedIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTime.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return Task.CompletedTask;
                }
            }
            user.LastLoggedIn = DateTime.UtcNow;
            _usersRepo.Update(user);
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

    public int GetCurrentUserId()
    {
        var claimsIdentity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
        var userId = userDataClaim?.Value;
        return string.IsNullOrWhiteSpace(userId)
            ? 0
            : int.Parse(userId, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public ValueTask<User> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        return FindUserAsync(userId);
    }

    public Task<(bool Succeeded, string Error)> ChangePasswordAsync(User user, string currentPassword,
        string newPassword)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            _unitOfWork.Begin();
            
            var currentPasswordHash = _securityService.GetSha256Hash(currentPassword);
            if (!string.Equals(user.Password, currentPasswordHash, StringComparison.Ordinal))
            {
                return Task.FromResult((false, "Current password is wrong."));
            }

            user.Password = _securityService.GetSha256Hash(newPassword);
            // user.SerialNumber = Guid.NewGuid().ToString("N"); // To force other logins to expire.
            _usersRepo.Update(user);
            _unitOfWork.Commit();
            return Task.FromResult((true, string.Empty));
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            Console.WriteLine(e);
            throw;
        }
    }
}