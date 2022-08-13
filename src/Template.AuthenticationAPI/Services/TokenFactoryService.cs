using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Template.AuthenticationAPI.Common;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Entities;

namespace Template.AuthenticationAPI.Services;

public class TokenFactoryService : ITokenFactoryService
{
    private readonly ILogger<TokenFactoryService> _logger;
    private readonly IRolesService _rolesService;
    private readonly ISecurityService _securityService;

    public TokenFactoryService(
        ISecurityService securityService,
        IRolesService rolesService,
        ILogger<TokenFactoryService> logger)
    {
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _rolesService = rolesService ?? throw new ArgumentNullException(nameof(rolesService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<JwtTokensData> CreateJwtTokensAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var (accessToken, claims) = await CreateAccessTokenAsync(user);
        var (refreshTokenValue, refreshTokenSerial) = CreateRefreshToken();
        return new JwtTokensData
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            RefreshTokenSerial = refreshTokenSerial,
            Claims = claims
        };
    }

    public string GetRefreshTokenSerial(string refreshTokenValue)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return null;
        }

        ClaimsPrincipal decodedRefreshTokenPrincipal = null;
        try
        {
            decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                refreshTokenValue,
                new TokenValidationParameters
                {
                    ValidIssuer = BearerTokensOptions.Issuer, // site that makes the token
                    ValidAudience = BearerTokensOptions.Audience, // site that consumes the token
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BearerTokensOptions.Key)),
                    ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                    ValidateLifetime = true, // validate the expiration
                    ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                },
                out _
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate refreshTokenValue: `{RefreshTokenValue}`", refreshTokenValue);
        }

        return decodedRefreshTokenPrincipal?.Claims
            ?.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.SerialNumber, StringComparison.Ordinal))?.Value;
    }

    private (string RefreshTokenValue, string RefreshTokenSerial) CreateRefreshToken()
    {
        var refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString()
            .Replace("-", "", StringComparison.Ordinal);

        var claims = new List<Claim>
        {
            // Unique Id for all Jwt tokes
            new(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(),
                ClaimValueTypes.String, BearerTokensOptions.Issuer),
            // Issuer
            new(JwtRegisteredClaimNames.Iss, BearerTokensOptions.Issuer, ClaimValueTypes.String,
                BearerTokensOptions.Issuer),
            // Issued at
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64, BearerTokensOptions.Issuer),
            // for invalidation
            new(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, BearerTokensOptions.Issuer)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BearerTokensOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            BearerTokensOptions.Issuer,
            BearerTokensOptions.Audience,
            claims,
            now,
            now.AddMinutes(BearerTokensOptions.RefreshTokenExpirationMinutes),
            creds);
        var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return (refreshTokenValue, refreshTokenSerial);
    }

    private async Task<(string AccessToken, IEnumerable<Claim> Claims)> CreateAccessTokenAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var claims = new List<Claim>
        {
            // Unique Id for all Jwt tokes
            new(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(),
                ClaimValueTypes.String, BearerTokensOptions.Issuer),
            // Issuer
            new(JwtRegisteredClaimNames.Iss, BearerTokensOptions.Issuer, ClaimValueTypes.String,
                BearerTokensOptions.Issuer),
            // Issued at
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64, BearerTokensOptions.Issuer),
            new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.String,
                BearerTokensOptions.Issuer),
            new(ClaimTypes.Name, user.Username, ClaimValueTypes.String, BearerTokensOptions.Issuer),
            new("DisplayName", user.DisplayName, ClaimValueTypes.String, BearerTokensOptions.Issuer),
            // to invalidate the cookie
            new(ClaimTypes.SerialNumber, user.SerialNumber, ClaimValueTypes.String, BearerTokensOptions.Issuer),
            // custom data
            new(ClaimTypes.UserData, user.Id.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.String,
                BearerTokensOptions.Issuer)
        };

        // add roles
        var roles = _rolesService.FindUserRolesAsync(user.Id);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName, ClaimValueTypes.String, BearerTokensOptions.Issuer));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BearerTokensOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            BearerTokensOptions.Issuer,
            BearerTokensOptions.Audience,
            claims,
            now,
            now.AddMinutes(BearerTokensOptions.AccessTokenExpirationMinutes),
            creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), claims);
    }
}